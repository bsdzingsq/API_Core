
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alipay.AopSdk.AspnetCore;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.WebApi.Utilities;
namespace ZsqApp.Core.WebApi.Controllers
{

    /// <summary>
    /// 支付宝充值回调
    /// </summary>
    [Produces("application/json")]
    [Route("api/AlipayNotify")]
    [EnableCors("any")]
    public class AlipayNotifyController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// 支付宝配置文件
        /// </summary>
        private readonly AliPaySetting _appSettings;
        /// <summary>
        /// 充值
        /// </summary>
        protected readonly IRecharge _recharge;
        /// <summary>
        /// 章鱼
        /// </summary>
        protected readonly IBiz _biz;
        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;

        /// <summary>
        /// 支付宝
        /// </summary>
        private readonly IAlipayService _alipayService;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="options"></param>
        /// <param name="recharge"></param>
        /// <param name="biz"></param>
        /// <param name="alipayService"></param>
        public AlipayNotifyController(IOptions<AliPaySetting> options, IRecharge recharge, IBiz biz, IAlipayService alipayService, IAccout accouts)
        {
            _appSettings = options.Value;
            _recharge = recharge;
            _biz = biz;
            _log = LogManager.GetLogger(Startup.repository.Name, typeof(AlipayNotifyController));
            _alipayService = alipayService;
            _accout = accouts;
        }
        #endregion


        /// <summary>
        /// 支付宝异步回调
        /// author:陶林辉
        /// </summary>
        /// <param name="sArray"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("notify")]
        public async Task Notify(Dictionary<string, string> sArray)
        {
            _log.Info($"支付宝回调{sArray}");
            AliPayOrderDto aliPayOrder = new AliPayOrderDto();
            if (sArray.Count != 0)
            {
                string out_trade_no = sArray["out_trade_no"];
                bool flag = _alipayService.RSACheckV1(sArray);
                if (flag)
                {
                    string status = "1";
                    //查询订单是否存在
                    aliPayOrder = await _recharge.GetAliPayOrderByIdAsync(out_trade_no);
                    //查询商品信息
                    var rechargeCommodity = await _recharge.GetRechargeCommodityByIdAsync(int.Parse(aliPayOrder.Commodity_id.ToString()));
                    if (aliPayOrder.Out_trade_no == "" && aliPayOrder.Out_trade_no == null)
                    {
                        /*订单不存在*/
                        return;
                    }
                    aliPayOrder.Out_trade_no = sArray["out_trade_no"]; //商户订单号                       
                    aliPayOrder.Fund_channel = sArray["fund_bill_list"];
                    aliPayOrder.Trade_no = sArray["trade_no"];//支付宝交易订单号
                    aliPayOrder.Gmt_payment = DateTime.Parse(sArray["gmt_payment"]); //付款时间
                    aliPayOrder.Updatetime = DateTime.Now;
                    aliPayOrder.Trade_status = sArray["trade_status"];
                    aliPayOrder.Buyer_id = sArray["buyer_logon_id"]; //买家支付宝账号
                    //更新数据库的支付宝订单状态
                    await _recharge.UpdateAlipayOrderAsync(aliPayOrder);
                    //判断支付状态
                    switch (aliPayOrder.Trade_status)
                    {
                        case "TRADE_SUCCESS":
                            status = "2";
                            break;
                        case "TRADE_CLOSED":
                            status = "3";
                            break;
                        case "WAIT_BUYER_PAY":
                            status = "1";
                            break;
                        case "TRADE_FINISHED":
                            status = "2";
                            break;
                        default:
                            break;

                    }
                    if (status=="2")
                    {
                        //关闭章鱼充值
                        // if (_biz.Recharge(aliPayOrder.Userid, aliPayOrder.Out_trade_no, double.Parse(rechargeCommodity.Amount.ToString()), "hiAlipay"))
                        if(_accout.Recharge_php(aliPayOrder.Userid, aliPayOrder.Out_trade_no, double.Parse(rechargeCommodity.Amount.ToString()), "充值", "yibiyibaidekey"))
                        {
                            
                            await _recharge.UpdateRechargeLogAsync(status, aliPayOrder.Out_trade_no);
                        }
                    }
                }
                else
                {
                    _log.Info($"{out_trade_no}验签名失败");
                }
            }
        }

    }
}