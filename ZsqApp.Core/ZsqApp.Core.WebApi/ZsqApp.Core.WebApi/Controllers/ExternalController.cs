using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.Channel;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.PHPRequest;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.Models.Routine;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.External;
using ZsqApp.Core.ViewModel.User;
using ZsqApp.Core.WebApi.Filters;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;

namespace ZsqApp.Core.WebApi.Controllers
{
    /// <summary>
    /// 给第三方服务端提供对外接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/External")]
    [ExternalActionApiFilterAttribute]
    [EnableCors("any")]
    public class ExternalController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// 系统
        /// </summary>
        protected readonly ISystems _sys;
        /// <summary>
        /// 认证
        /// </summary>
        protected readonly IToken _token;
        /// <summary>
        /// 章鱼
        /// </summary>
        private readonly IBiz _biz;
        /// <summary>
        /// 用户
        /// </summary>
        protected readonly IUser _user; /**/
        /// <summary>
        /// log4
        /// </summary>
        protected readonly ILog _log;
        /// <summary>
        /// 充值
        /// </summary>
        protected readonly IRecharge _recharge;
        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;
        /// <summary>
        /// 海峡竞技
        /// </summary>
        private readonly IOptions<HaiXiaSetting> _options;

        private readonly IRoutine _routine;
        /// <summary>
        /// 获取渠道树
        /// </summary>
        private readonly IChannel _channel;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="token"></param>
        /// <param name="biz"></param>
        /// <param name="user"></param>
        /// <param name="recharge"></param>
        /// <param name="accout"></param>
        /// <param name="options"></param>
        /// <param name="routine"></param>
        public ExternalController(ISystems sys, IToken token, IBiz biz, IUser user, IRecharge recharge, IAccout accout, IOptions<HaiXiaSetting> options, IRoutine routine, IChannel channel)
        {
            _sys = sys;
            _token = token;
            _biz = biz;
            _user = user;
            _log = LogManager.GetLogger(Startup.repository.Name, typeof(ExternalController));
            _recharge = recharge;
            _accout = accout;
            _options = options;
            _routine = routine;
            _channel = channel;
        }
        #endregion

        /// <summary>
        /// 非APP端用户认证
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("tokenverify")]
        public ExternalResponesViewModel<TokenverifyViewModel> Tokenverify([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<TokenverifyViewModel> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            TokenverifyViewModel result = new TokenverifyViewModel();
            string openId = obj.Data.userOpenId;
            string sessionToken = obj.Data.sessionToken;
            bool isLog = _token.VerifyToken(openId, sessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{openId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                result.UserId = UserLog.Userid;
            }
            else
            {
                sysCode = SysCode.SessionTokenLose;
                result.UserId = 0;
            }

            respones = new ExternalResponesViewModel<TokenverifyViewModel>(sysCode, _sys, result);
            return respones;
        }

        /// <summary>
        /// 非APP端用户认证
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userverify")]
        public ExternalResponesViewModel<TokenverifyViewModel> userverify([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<TokenverifyViewModel> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            TokenverifyViewModel result = new TokenverifyViewModel();
            var sysCode = SysCode.Ok;
            string token = obj.Data.token;
            bool fa = _token.VerifyToken(token);
            if (fa)
            {
                string json = RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five);
                UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                result.UserId = userLogin.Userid;
            }
            else
            {
                sysCode = SysCode.TokenLose;
                result.UserId = 0;
            }
            respones = new ExternalResponesViewModel<TokenverifyViewModel>(sysCode, _sys, result);
            return respones;
        }

        /// <summary>
        /// 获取用户信息
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserInfo")]
        public async Task<ExternalResponesViewModel<UserInfoView>> GetUserInfo([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<UserInfoView> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            UserInfoView result = new UserInfoView();
            long userId = long.Parse((string)obj.Data.userId);
            if (!await _user.JudgeUserIdIsNoAsync(userId))
            {
                //用户不存在
                sysCode = SysCode.UserExist;
            }
            if (sysCode == SysCode.Ok)
            {
                UserInfoDto UserInfo = await _user.SearchUserInfoAsync(userId);
                UserBalanceDto UserBalance = null;

                //关闭章鱼余额查询
                //UserBalance = _biz.AcquireBalance(userId);
                //从php获取用户余额
                UserBalance = _accout.AcquireBalance_php(userId);

                UserLoginDto userLogin = await _user.GetUserLoginAsync(userId);
                //查询用户渠道并返回用户信息
                var userchannel = await _routine.GetUserIdChannelIdAsync(userId);
                result = new UserInfoView
                {
                    Balance = UserBalance.Balance,
                    IdCard = UserInfo.Id_card,
                    Nickname = UserInfo.Nick_name,
                    Phone = userLogin.Phone,
                    PrizeBalance = UserBalance.PrizeBalance,
                    RealName = UserInfo.Real_name,
                    CreateTime = UserInfo.Createtime,
                    channel = userchannel
                };
            }
            respones = new ExternalResponesViewModel<UserInfoView>(sysCode, _sys, result);
            return respones;
        }

        /// <summary>
        /// 商品兑换
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("exchange")]
        public async Task<ExternalResponesViewModel<object>> Exchange([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isNo = false;
            if (RedisHelper.KeyExists($"{CacheKey.Token}{obj.Data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three))
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.Data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                ExchangeDto exchange = new ExchangeDto
                {
                    Amount = double.Parse((string)obj.Data.amount),
                    ForderId = (string)obj.Data.forderId,
                    FuserId = UserLog.Userid.ToString(),
                    Price = Double.Parse((string)obj.Data.price),
                    Quantity = int.Parse((string)obj.Data.quantity),
                    Name = "商城兑出"

                };
                //关闭章鱼兑换
                //isNo = await _biz.Exchange(exchange);
                //从php兑换
                isNo = await _accout.Exchange_php(exchange);

                sysCode = isNo ? SysCode.Ok : SysCode.Err;
            }
            else
            {
                //不存在的openid
                sysCode = SysCode.UserOpenIdisNo;
            }
            respones = new ExternalResponesViewModel<object>(sysCode, _sys, null);
            return respones;
        }

        /// <summary>
        /// 商品兑换
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("exchange_h5")]
        public async Task<ExternalResponesViewModel<object>> Exchange_H5([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            //bool isNo = false;
            if (RedisHelper.KeyExists($"{CacheKey.Token}{obj.Data.token}", RedisFolderEnum.token, RedisEnum.Five))
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.Data.token}", RedisFolderEnum.token, RedisEnum.Five);
                UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                ExchangeDto exchange = new ExchangeDto
                {
                    Amount = double.Parse((string)obj.Data.amount),
                    ForderId = (string)obj.Data.forderId,
                    FuserId = userLogin.Userid.ToString(),
                    Price = Double.Parse((string)obj.Data.price),
                    Quantity = int.Parse((string)obj.Data.quantity),
                    Name = "商城兑出"

                };

                //关闭章鱼兑换
                // bool isNo = await _biz.Exchange(exchange);
                //从php兑出
                bool isNo = await _accout.Exchange_php(exchange);

                sysCode = isNo ? SysCode.Ok : SysCode.Err;
                _log.InfoFormat("商品兑换，参数:{0},兑换结果{1}", JsonHelper.SerializeObject(exchange), isNo);
            }
            else
            {
                //不存在的openid
                sysCode = SysCode.UserOpenIdisNo;
            }
            respones = new ExternalResponesViewModel<object>(sysCode, _sys, null);
            return respones;
        }

        /// <summary>
        /// 订单退款
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("refund")]
        public async Task<ExternalResponesViewModel<object>> Refund([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            RefundDto refund = new RefundDto
            {
                description = (string)obj.Data.description,
                forderId = (string)obj.Data.forderId
            };
            //关闭章鱼退款
            //bool isNo = await _biz.Refund(refund);
            //从pho退款
            bool isNo = await _accout.Refund_php(refund);
            _log.InfoFormat("订单退款，参数:{0},兑换结果{1}", JsonHelper.SerializeObject(refund), isNo);
            sysCode = isNo ? SysCode.Ok : SysCode.Err;
            respones = new ExternalResponesViewModel<object>(sysCode, _sys, null);
            return respones;
        }


        /// <summary>
        /// 对外充值
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("recharge")]
        public async Task<ExternalResponesViewModel<object>> Recharge([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            if (!await _user.JudgeUserIdIsNoAsync(long.Parse((string)obj.Data.userId)))
            {
                //用户不存在
                sysCode = SysCode.UserExist;
            }
            //判断充值订单是否存在并且充值成功
            //如订单存在充值失败，重新充值
            //如订单存在充值成功，错误提示订单以充值成功
            if (await _recharge.JuderOrderIsSuccess((string)obj.Data.orderId))
            {
                //订单号存在
                sysCode = SysCode.OrderIsSuccess;
            }
            if (sysCode == SysCode.Ok)
            {
                if (await _recharge.JuderOrderIsimplemeAsync((string)obj.Data.orderId))
                {
                    /*订单充值中*/
                    sysCode = SysCode.OrderIsimplement;
                }
            }
            if (sysCode == SysCode.Ok)
            {
                RechargeDto recharge = new RechargeDto()
                {
                    Amount = obj.Data.amount,
                    createtime = DateTime.Now,
                    Order_id = (string)obj.Data.orderId,
                    Pay_type = int.Parse((string)obj.Data.payType),//0苹果内购， 1支付宝，2微信 ，3卡密
                    Status = 1, //1 待处理(充值中) 2 已支付 3 充值失败 4 超时
                    UserId = long.Parse((string)obj.Data.userId),
                    updatetime = DateTime.Now,
                };
                /*提前在充值前记录订单*/
                await _recharge.RecordRechargeLogAsync(recharge);
                double amount = double.Parse(recharge.Amount.ToString());
                bool isNo = false;

                //关闭章鱼充值
                //_biz.Recharge(recharge.UserId, recharge.Order_id, amount, "hiAlipay");
                //从php充值
                isNo = _accout.Recharge_php(recharge.UserId, recharge.Order_id, amount, "充值", "yibiyibaidekey");


                if (isNo)
                {
                    /*充值成功*/
                    recharge.Status = 2;
                    sysCode = SysCode.Ok;
                }
                else
                {
                    /*充值失败*/
                    recharge.Status = 3;
                    sysCode = SysCode.Err;
                }
                /*充值完成后更新充值记录*/
                await _recharge.UpdateRechargeLogAsync(recharge.Status.ToString(), recharge.Order_id);
                _log.InfoFormat("账户充值，参数:{0},充值结果{1}", JsonHelper.SerializeObject(recharge), isNo);
            }
            respones = new ExternalResponesViewModel<object>(sysCode, _sys, null);
            return respones;
        }

        /// <summary>
        /// 根据手机号码获取用户id
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getuseridbyPhone")]
        public async Task<ExternalResponesViewModel<TokenverifyViewModel>> GetUserIdByPhone([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<TokenverifyViewModel> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            TokenverifyViewModel tokenverify = new TokenverifyViewModel();
            var sysCode = SysCode.Ok;
            if (await _user.PhoneExistAsync((string)obj.Data.phone))
            {
                tokenverify.UserId = _user.GetUserIdByPhone((string)obj.Data.phone);
            }
            else
            {
                sysCode = SysCode.PhoneNonentity;
            }
            respones = new ExternalResponesViewModel<TokenverifyViewModel>(sysCode, _sys, sysCode == SysCode.Ok ? tokenverify : null);
            return respones;

        }

        /// <summary>
        /// 微信充值获取商品信息和用户userId
        /// author:林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("weChatInfo")]
        public async Task<ExternalResponesViewModel<WeChatRecharge>> WeChatInfo([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<WeChatRecharge> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            WeChatRecharge weChatRecharge = new WeChatRecharge();
            var sysCode = SysCode.Ok;
            string token = obj.Data.token;
            bool isNo = _token.VerifyToken(token);
            if (isNo)
            {
                UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                long userId = long.Parse(userLogin.Userid.ToString());
                int id = StringExtension.ToInt((string)obj.Data.rechargeId);
                var rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(id);
                if (rechargeCommodity == null)
                {
                    sysCode = SysCode.IdIsNull;
                }
                if (sysCode == SysCode.Ok)
                {
                    weChatRecharge.userId = userId;
                    weChatRecharge.amount = rechargeCommodity.Amount;
                    weChatRecharge.money = rechargeCommodity.Money;
                    weChatRecharge.name = rechargeCommodity.Name;
                    weChatRecharge.phone = userLogin.Phone;
                }
            }
            else
            {
                sysCode = SysCode.TokenLose;
            }
            respones = new ExternalResponesViewModel<WeChatRecharge>(sysCode, _sys, sysCode == SysCode.Ok ? weChatRecharge : null);
            return respones;

        }

        /// <summary>
        /// 海峡竞技消费
        /// author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Competitive_exchange")]
        public async Task<ExternalResponesViewModel<object>> Competitive_exchange([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            bool isNo = false;
            string openId = obj.Data.userOpenId;
            string sessionToken = obj.Data.sessionToken;
            bool isLog = _token.VerifyToken(openId, sessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{openId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                if (UserLog.Userid == Convert.ToInt64(obj.Data.userId))
                {
                    //海峡订单消费
                    ConsumeDto consume = new ConsumeDto
                    {
                        forderId = (string)obj.Data.forderId,
                        fuserId = UserLog.Userid.ToString(),
                        amount = double.Parse((string)obj.Data.amount),
                        description = obj.Data.name
                    };
                    //关闭章鱼调用海峡竞技消费
                    //isNo = await _accout.Consume(consume);
                    isNo = await _accout.Consume_php(consume);
                    UserBalanceDto UserBalanceNow = null;
                    UserBalanceNow = _accout.AcquireBalance_php(UserLog.Userid);
                    sysCode = isNo ? SysCode.Ok : SysCode.Err;
                    _log.InfoFormat("海峡订单消费后余额，参数:{0},兑换结果{1},消费前余额{2}消费后余额{3}", JsonHelper.SerializeObject(consume), isNo, JsonHelper.SerializeObject(obj.Data), JsonHelper.SerializeObject(UserBalanceNow));
                }
                else
                {
                    sysCode = SysCode.UserIsNo;
                }
            }
            else
            {
                sysCode = SysCode.SessionTokenLose;
            }
            respones = new ExternalResponesViewModel<object>(sysCode, _sys, null);
            return respones;
        }

        /// <summary>
        /// 获取用户的充值类型
        ///  author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetRechangeType")]
        public async Task<ExternalResponesViewModel<List<RechargeTypeDto>>> GetRechangeType([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<List<RechargeTypeDto>> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            string orderList = obj.Data.OrdersList;
            List<RechargeTypeDto> rechargeList = new List<RechargeTypeDto>();
            var results = _accout.GetRechargeTypes(Convert.ToString(orderList));
            foreach (var item in results)
            {
                RechargeTypeDto rechargeType = new RechargeTypeDto();
                rechargeType.UserId = item.UserId.IsBlank() ? -1 : long.Parse(item.UserId);
                rechargeType.Order_id = item.OrderId;
                rechargeType.Pay_type = item.PayType;
                rechargeList.Add(rechargeType);
            }
            string[] strOrder = orderList.Split(',');
            foreach (var item in strOrder)
            {
                var result = await _recharge.GetRechargeAsync(item);
                if (result != null)
                {
                    rechargeList.Add(result);
                }
            }
            respones = new ExternalResponesViewModel<List<RechargeTypeDto>>(sysCode, _sys, sysCode == SysCode.Ok ? rechargeList : null);
            return respones;
        }

        /// <summary>
        /// 518与签到类别
        ///  author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetCoinDonation")]
        public async Task<ExternalResponesViewModel<List<GiveCurrencyLogDto>>> GetCoinDonation([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<List<GiveCurrencyLogDto>> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            List<GiveCurrencyLogDto> giveCurrency = new List<GiveCurrencyLogDto>();
            var sysCode = SysCode.Ok;
            string signList = obj.Data.SignList;
            string[] strSign = signList.Split(',');
            foreach (var item in strSign)
            {
                var result = await _recharge.GetSignAsync(item);
                if (result != null)
                {
                    giveCurrency.Add(result);
                }
            }
            respones = new ExternalResponesViewModel<List<GiveCurrencyLogDto>>(sysCode, _sys, sysCode == SysCode.Ok ? giveCurrency : null);
            return respones;
        }

        /// <summary>
        /// 根据用户id查询渠道(先查ibc渠道哦再查用户渠道表没有其次查用户注册渠道)
        /// author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserIdChannel")]
        public async Task<ExternalResponesViewModel<object>> GetUserIdChannel([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<object> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            long userid = (long)obj.Data.userId;
            DateTime times = DateTime.Parse((string)obj.Data.stime);
            UserChannelDto userChannel = new UserChannelDto();
            var ibcChannel = _routine.GetIbcChannel(Convert.ToString(userid), times);
            var userchannel = await _routine.GetUserIdChannelIdAsync(userid);
            if (ibcChannel != null)
            {
                userChannel.ChannelId = ibcChannel;
                userChannel.ChannelType = "1"; //1 为ibc渠道 2 为用户渠道或注册渠道
            }
            else if (userchannel != null)
            {
                userChannel.ChannelId = userchannel;
                userChannel.ChannelType = "2";
            }
            else
            {
                sysCode = SysCode.IdIsNull;
            }

            respones = new ExternalResponesViewModel<object>(sysCode, _sys, sysCode == SysCode.Ok ? userChannel : null);
            return respones;
        }

        /// <summary>
        /// 获取渠道树
        /// author:白尚德</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetChannelTree")]
        public async Task<ExternalResponesViewModel<ChnnelList>> GetChannelTree([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<ChnnelList> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            string strKey = obj.Data.key;
            ChnnelList channelTree = new ChnnelList();
            var reandom = _sys.GetRandom();
            if (strKey.IsBlank())
            {

                channelTree.ChnneltreesList = await _channel.GetChnnelIbcAsync();
                channelTree.key = reandom;
                RedisHelper.StringSet(reandom, channelTree, 10080, RedisFolderEnum.ChannelTree, RedisEnum.Nine);
            }
            else if (strKey.IsNotBlank())
            {
                if (RedisHelper.KeyExists(strKey, RedisFolderEnum.ChannelTree, RedisEnum.Nine))
                {
                    sysCode = SysCode.DataisNoUpt;
                }
                else
                {
                    channelTree.ChnneltreesList = await _channel.GetChnnelIbcAsync();
                    channelTree.key = reandom;
                    RedisHelper.StringSet(reandom, channelTree, 10080, RedisFolderEnum.ChannelTree, RedisEnum.Nine);
                }
            }
            else
            {
                sysCode = SysCode.ErrParameter;
            }
            respones = new ExternalResponesViewModel<ChnnelList>(sysCode, _sys, sysCode == SysCode.Ok ? channelTree : null);
            return respones;
        }

        /// <summary>
        /// 根据时间获取各个渠道注册人数
        /// author:白尚德</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserNumber")]
        public async Task<ExternalResponesViewModel<List<RegisterNumberDto>>> GetUserNumber([FromBody]ExternalRequesViewModel obj)
        {
            ExternalResponesViewModel<List<RegisterNumberDto>> respones = null;
            obj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(Content(User.Identity.Name).Content);
            List<RegisterNumberDto> userCount = null;
            var sysCode = SysCode.Ok;

            DateTime startTime = DateTime.Parse((string)obj.Data.StartTime);
            DateTime overTime = DateTime.Parse((string)obj.Data.OverTime);
            userCount = await _user.GetResignCountAsync(startTime, overTime);
            if (userCount.Count == 0)
            {
                sysCode = SysCode.IdIsNull;
            }

            respones = new ExternalResponesViewModel<List<RegisterNumberDto>>(sysCode, _sys, sysCode == SysCode.Ok ? userCount : null);
            return respones;
        }


        //end
    }
}