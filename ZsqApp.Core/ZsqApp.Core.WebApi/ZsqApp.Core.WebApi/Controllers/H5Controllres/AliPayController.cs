using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.Recharge;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{
    /// <summary>
    /// H5的支付宝支付
    /// </summary>
    [Produces("application/json")]
    [Route("api/AliPay")]
    [EnableCors("any")]
    public class AliPayController : Controller
    {

        #region  DI
        /// <summary>
        /// 用户
        /// </summary>
        protected readonly IUser _user;

        /// <summary>
        /// 系统
        /// </summary>
        protected readonly ISystems _sys;

        /// <summary>
        /// IRecharge 充值相关服务
        /// </summary>
        protected readonly IRecharge _recharge;

        /// <summary>
        /// IBiz 章鱼相关服务
        /// </summary>
        protected readonly IBiz _biz;

        /// <summary>
        /// 令牌
        /// </summary>
        private readonly IToken _token;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sys"></param>
        /// <param name="recharge"></param>
        /// <param name="biz"></param>
        /// <param name="token"></param>
        public AliPayController(IUser user, ISystems sys, IRecharge recharge, IBiz biz, IToken token)
        {
            _biz = biz;
            _recharge = recharge;
            _sys = sys;
            _user = user;
            _token = token;
        }

        #endregion

        /// <summary>
        /// h5支付宝充值获取充值列表
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost, Route("acquireRechargecommodity")]
        public async Task<H5ResponseViewModel<CommodityListView>> AcquireRechargeCommodity([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<CommodityListView> response = null;
            CommodityListView result = new CommodityListView();
            var sysCode = _sys.CheckParameters(Parameters.data, "login");
            result.CommodityList = await _recharge.GetRechargeCommodityAsync(0);
            response = new H5ResponseViewModel<CommodityListView>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// 发起支付请求
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost, Route("payrequest")]
        public async Task<H5ResponseViewModel<AliPayH5View>> PayRequest([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "payrequest");
            H5ResponseViewModel<AliPayH5View> response = null;
            AliPayH5View result = null;
            string token = Parameters.data.token;
            bool isLog = _token.VerifyToken(token);
            if (sysCode == SysCode.Ok)
            {
                if (isLog)
                {
                    string json = RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five);
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(json);
                    int id = int.Parse((string)Parameters.data.id);
                    var rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(id);
                    string order = Guid.NewGuid().ToString();
                    result = _recharge.CreateAlipayH5Receipt(rechargeCommodity, order);
                    AliPayOrderDto AliPayOrder = new AliPayOrderDto()
                    {
                        Commodity_id = id,
                        Out_trade_no = order,
                        Createtime = DateTime.Now,
                        Updatetime = DateTime.Now,
                        Gmt_payment = DateTime.Now,
                        Userid = userLogin.Userid,
                        Total_amount = rechargeCommodity.Money //人名币
                    };
                    await _recharge.RecordAlipayOrderAsync(AliPayOrder);//记录支付宝订单。
                    RechargeDto recharge = new RechargeDto()
                    {
                        Amount = rechargeCommodity.Amount, //虚拟币
                        createtime = DateTime.Now,
                        Pay_type = 1, //支付类型 1支付宝，2微信,3苹果内购
                        Status = 1, //1 待处理 2 已支付 3 支付失败 4 超时
                        updatetime = DateTime.Now,
                        UserId = userLogin.Userid,
                        Order_id = AliPayOrder.Out_trade_no,
                    };
                    await _recharge.RecordRechargeLogAsync(recharge); //记录充值订

                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<AliPayH5View>(sysCode, result ?? null);
            return response;
        }

    }
}