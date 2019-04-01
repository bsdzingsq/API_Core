using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.Recharge;
using ZsqApp.Core.WebApi.Filters;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;
using static ZsqApp.Core.WebApi.Model.WeCharRequest;

namespace ZsqApp.Core.WebApi.Controllers
{
    /// <summary>
    /// 充值相关
    /// </summary>
    [ActionApiFilterAttribute]
    [EnableCors("any")]
    [Produces("application/json")]
    [Route("api/recharge")]
    public class RechargeController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// IToken 用户token服务
        /// </summary>
        private readonly IToken _token;
        /// <summary>
        /// IUser 用户相关服务
        /// </summary>
        protected readonly IUser _user;
        /// <summary>
        /// ISystems 系统相关服务
        /// </summary>
        protected readonly ISystems _sys;
        /// <summary>
        /// IRecharge 充值相关服务
        /// </summary>
        protected readonly IRecharge _recharge;
        /// <summary>
        /// IRoutine APP相关服务
        /// </summary>
        private readonly IRoutine _routine;
        /// <summary>
        /// IBiz 章鱼相关服务
        /// </summary>
        protected readonly IBiz _biz;
        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;
        private readonly WeChatPaySetting _weChatPay;
        private readonly LqhnWeChatPaySetting _lqhnweChatPay;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="sys"></param>
        /// <param name="recharge"></param>
        /// <param name="routine"></param>
        /// <param name="biz"></param>
        public RechargeController(IUser user, IToken token, IOptions<WeChatPaySetting> weChatPay, IOptions<LqhnWeChatPaySetting> lqhnweChatPay, ISystems sys, IRecharge recharge, IRoutine routine, IBiz biz, IAccout accout)
        {
            _sys = sys;
            _user = user;
            _token = token;
            _recharge = recharge;
            _routine = routine;
            _biz = biz;
            _accout = accout;
            _weChatPay = weChatPay.Value;
            _lqhnweChatPay = lqhnweChatPay.Value;
        }
        #endregion

        /// <summary>
        /// 获取充值商品列表
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquirerechargecommodity")]
        public async Task<ResponseViewModel<CommodityListView>> AcquireRechargeCommodity([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<CommodityListView> response = null;
            CommodityListView result = new CommodityListView();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            var type = 0;
            if (await _routine.JudgeVersionAuditStatusAsync(obj.Client.Version))//app是否在审核期
            {
                type = 1;
            }
            result.CommodityList = await _recharge.GetRechargeCommodityAsync(type);
            response = new ResponseViewModel<CommodityListView>(sysCode, result.CommodityList.Count > 0 ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取支付宝订单信息
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireAliPaySign")]
        public async Task<ResponseViewModel<string>> AcquireAliPaySign([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<string> response = null;
            string result = string.Empty;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string json = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                int itemId = StringExtension.ToInt((string)obj.Data.itemId); //; //商品Id
                RechargeCommodityDto rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(itemId);
                if (rechargeCommodity == null)
                {
                    sysCode = SysCode.RechargeCommodityIsNULL; //充值商品不存在，
                }
                if (sysCode == SysCode.Ok)
                {
                    string order = Guid.NewGuid().ToString();
                    result = _recharge.CreateAlipayReceipt(rechargeCommodity, order);
                    AliPayOrderDto AliPayOrder = new AliPayOrderDto()
                    {
                        Commodity_id = itemId,
                        Out_trade_no = order,
                        Createtime = DateTime.Now,
                        Updatetime = DateTime.Now,
                        Gmt_payment = DateTime.Now,
                        Userid = userLog.Userid,
                        Total_amount = rechargeCommodity.Money
                    };
                    await _recharge.RecordAlipayOrderAsync(AliPayOrder);//记录支付宝订单。
                    RechargeDto recharge = new RechargeDto()
                    {
                        Amount = rechargeCommodity.Amount,
                        createtime = DateTime.Now,
                        Pay_type = 1, //支付类型 1支付宝，2微信,3苹果内购
                        Status = 1, //1 待处理 2 已支付 3 支付失败 4 超时
                        updatetime = DateTime.Now,
                        UserId = userLog.Userid,
                        Order_id = AliPayOrder.Out_trade_no,
                    };
                    await _recharge.RecordRechargeLogAsync(recharge); //记录充值订
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<string>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取用户的充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireUserRechargeList")]
        public async Task<ResponseViewModel<List<RechargeDto>>> AcquireUserRechargeList([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<List<RechargeDto>> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            List<RechargeDto> result = new List<RechargeDto>();
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                result = await _recharge.GetRechargeListAsync(userLog.Userid);
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<List<RechargeDto>>(sysCode, result.Count > 0 ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// IOS创建iap支付订单
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireAppleProduct")]
        public async Task<ResponseViewModel<AppleProductViewModel>> AcquireAppleProduct([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<AppleProductViewModel> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            AppleProductViewModel result = null;
            if (isLog)
            {
                int itemId = int.Parse((string)obj.Data.itemId);
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                var dto = await _recharge.GetAppleProductAsync(itemId); //查询苹果商品信息
                if (dto != null)
                {
                    /*苹果充值记录*/
                    AppleReceiptDto appleReceipt = new AppleReceiptDto()
                    {
                        Apple_id = dto.Product_id,
                        Item_id = itemId,
                        Createtime = DateTime.Now,
                        Receipt = "",
                        Userid = userLog.Userid,
                        Updatetime = DateTime.Now,
                        Order_id = Guid.NewGuid().ToString(),

                    };
                    await _recharge.RecordAppleOrderAsync(appleReceipt);
                    /*用户充值记录*/
                    RechargeDto recharge = new RechargeDto()
                    {
                        Amount = dto.Amount,
                        createtime = DateTime.Now,
                        Order_id = appleReceipt.Order_id,
                        Pay_type = 0,//0苹果内购， 1支付宝，2微信 ，3卡密
                        Status = 1, //1 待处理 2 已支付 3 支付失败 4 超时
                        UserId = userLog.Userid,
                        updatetime = DateTime.Now,
                    };
                    await _recharge.RecordRechargeLogAsync(recharge);
                    result = new AppleProductViewModel
                    {
                        AppleId = dto.Apple_id,
                        ProductId = dto.Product_id,
                        OrderId = recharge.Order_id,//记录充值订
                    };
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }

            response = new ResponseViewModel<AppleProductViewModel>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 上传并认证IOS收据
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("uploadingReceipt")]
        public async Task<ResponseViewModel<object>> UploadingReceipt([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                ReceiptDto receipt = new ReceiptDto
                {
                    AppleId = (string)obj.Data.appleId,
                    Id = (string)obj.Data.productId,
                    OrderId = (string)obj.Data.orderId,
                    Receipt = (string)obj.Data.receipt
                };
                AppleReceiptDto appleReceipt = new AppleReceiptDto()
                {
                    Updatetime = DateTime.Now,
                    Receipt = receipt.Receipt,
                    Order_id = receipt.OrderId,
                };
                if (_recharge.VerifyReceipt(receipt))
                {
                    //获取商品信息
                    RechargeCommodityDto rechargeCommodity = await _recharge.GetAppleProductAsync(receipt.AppleId);
                    //充值虚拟币
                    double amount = double.Parse(rechargeCommodity.Amount.ToString());

                    //关闭章鱼充值
                    //if (_accout.Recharge_php(userLog.Userid, receipt.OrderId, amount, "充值", "hiAlipay"))
                    //从php充值
                    if (_accout.Recharge_php(userLog.Userid, receipt.OrderId, amount, "充值", "yibiyibaidekey"))
                    {
                       
                        //更新苹果订单
                        await _recharge.UpdateAppleOrderAsync(appleReceipt);
                        //更新充值记录
                        await _recharge.UpdateRechargeLogAsync("2", receipt.OrderId);
                    }
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }


        /// <summary>
        /// 获取微信订单信息
        /// author：白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("weCharPaySign")]
        public async Task<ResponseViewModel<WeChatResponse>> WeCharPaySign([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<WeChatResponse> response = null;
            WeChatResponse result = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string json = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                int itemId = StringExtension.ToInt((string)obj.Data.itemId); //; //商品Id
                RechargeCommodityDto rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(itemId);
                if (rechargeCommodity == null)
                {
                    sysCode = SysCode.RechargeCommodityIsNULL; //充值商品不存在，
                }
                if (sysCode == SysCode.Ok)
                {
                    string strUrl = _weChatPay.Url;
                    WeCharRequest weChar = new WeCharRequest();
                    weChar.userOpenId = obj.HendInfo.UserOpenId;
                    weChar.itemId = (string)obj.Data.itemId;
                    weChar.sessionToken = obj.HendInfo.SessionToken;
                    WeCharRequestList requestList = new WeCharRequestList();
                    requestList.data = weChar;
                    string strJson = JsonHelper.SerializeObject(requestList);
                    result = JsonHelper.DeserializeJsonToObject<WeChatResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<WeChatResponse>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取支付宝订单信息
        /// author：白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AliPaySign")]
        public async Task<ResponseViewModel<string>> AliPaySign([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<string> response = null;
            string result = string.Empty;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string json = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                int itemId = StringExtension.ToInt((string)obj.Data.itemId); //; //商品Id
                RechargeCommodityDto rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(itemId);
                if (rechargeCommodity == null)
                {
                    sysCode = SysCode.RechargeCommodityIsNULL; //充值商品不存在，
                }
                if (sysCode == SysCode.Ok)
                {
                    string order = Guid.NewGuid().ToString();
                    result = _recharge.CreateAlipaySign(rechargeCommodity, order);
                    AliPayOrderDto AliPayOrder = new AliPayOrderDto()
                    {
                        Commodity_id = itemId,
                        Out_trade_no = order,
                        Createtime = DateTime.Now,
                        Updatetime = DateTime.Now,
                        Gmt_payment = DateTime.Now,
                        Userid = userLog.Userid,
                        Total_amount = rechargeCommodity.Money
                    };
                    await _recharge.RecordAlipayOrderAsync(AliPayOrder);//记录支付宝订单。
                    RechargeDto recharge = new RechargeDto()
                    {
                        Amount = rechargeCommodity.Amount,
                        createtime = DateTime.Now,
                        Pay_type = 1, //支付类型 1支付宝，2微信,3苹果内购
                        Status = 1, //1 待处理 2 已支付 3 支付失败 4 超时
                        updatetime = DateTime.Now,
                        UserId = userLog.Userid,
                        Order_id = AliPayOrder.Out_trade_no,
                    };
                    await _recharge.RecordRechargeLogAsync(recharge); //记录充值订
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<string>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取乐趣海南微信订单信息
        /// author：白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireweCharPaySign")]
        public async Task<ResponseViewModel<WeChatResponse>> AcquireWeCharPaySign([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<WeChatResponse> response = null;
            WeChatResponse result = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string json = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                int itemId = StringExtension.ToInt((string)obj.Data.itemId); //; //商品Id
                RechargeCommodityDto rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(itemId);
                if (rechargeCommodity == null)
                {
                    sysCode = SysCode.RechargeCommodityIsNULL; //充值商品不存在，
                }
                if (sysCode == SysCode.Ok)
                {
                    string strUrl = _lqhnweChatPay.Url;
                    WeCharRequest weChar = new WeCharRequest();
                    weChar.userOpenId = obj.HendInfo.UserOpenId;
                    weChar.itemId = (string)obj.Data.itemId;
                    weChar.sessionToken = obj.HendInfo.SessionToken;
                    WeCharRequestList requestList = new WeCharRequestList();
                    requestList.data = weChar;
                    string strJson = JsonHelper.SerializeObject(requestList);
                    result = JsonHelper.DeserializeJsonToObject<WeChatResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<WeChatResponse>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }
    }
}