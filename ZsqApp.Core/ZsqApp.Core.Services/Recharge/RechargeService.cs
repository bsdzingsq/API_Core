using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Util;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Entity.Recharge;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Recharge;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.ViewModel.Recharge;

namespace ZsqApp.Core.Services.Recharge
{
    public class RechargeService : IRecharge
    {
        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        //private readonly IOptions<AliPaySetting> _options;
        private readonly AliPaySetting _appSettings;
        private readonly ISystems _sys;
        private readonly ApplepaySetting _applepay;
        private readonly IAlipayService _alipayService;
        private readonly StraitAliPaySetting _straitAliPay;


        public RechargeService(IMapper mapper, FunHaiNanContext context, IOptions<StraitAliPaySetting> straitAliPay, IOptions<AliPaySetting> options, ISystems sys, IOptions<ApplepaySetting> optionsApple, IAlipayService alipayService)
        {
            _context = context;
            _mapper = mapper;
            //_options = options;
            _appSettings = options.Value;
            _sys = sys;
            _applepay = optionsApple.Value;
            _alipayService = alipayService;
            _straitAliPay = straitAliPay.Value;
        }

        /// <summary>
        /// 获取充值商品列表
        /// author:陶林辉
        /// </summary>
        /// <param name="type">商品类型 0普通，1ios审核商品</param>
        /// <returns></returns>
        public async Task<List<RechargeCommodityDto>> GetRechargeCommodityAsync(int type)
        {
            var result = await _context.RechargeCommodity.Where(m => m.Is_status == 1 && m.Type == type).OrderByDescending(t => t.Rank).ToListAsync();
            return _mapper.Map<List<RechargeCommodityEntity>, List<RechargeCommodityDto>>(result);
        }

        /// <summary>
        /// 根据Id获取单个充值商品
        /// author:陶林辉
        /// </summary>
        /// <param name="id">商品id</param>
        /// <returns></returns>
        public async Task<RechargeCommodityDto> GetRechargeCommodityByIdAsync(int id)
        {
            var result = await _context.RechargeCommodity.Where(m => m.Id == id).FirstOrDefaultAsync();
            return _mapper.Map<RechargeCommodityEntity, RechargeCommodityDto>(result);
        }

        /// <summary>
        /// 生成支付宝订单
        /// author:陶林辉
        /// </summary>
        /// <param name="recharge">商品对象</param>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        public string CreateAlipayReceipt(RechargeCommodityDto recharge, string order)
        {

            string privateKeyPem = _appSettings.PrivateKeyPem;//商户私钥
            string out_trade_no = order;//订单号
            double total_fee = double.Parse(recharge.Money.ToString());//交易金额
            string app_id = _appSettings.App_id;//app支付，支付宝中该应用的ID
            string charset = "utf-8";//utf-8
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string notify_url = _appSettings.Notify_url;//回调地址
            string body = recharge.Name;
            string subject = "乐趣海南充值";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            dic.Add("app_id", app_id);
            dic.Add("method", "alipay.trade.app.pay");
            dic.Add("version", "1.0");
            dic.Add("charset", charset);
            dic.Add("notify_url", notify_url);
            dic.Add("sign_type", _appSettings.Sign_type);
            dic.Add("timestamp", timestamp);
            dic2.Add("out_trade_no", out_trade_no);//商户订单号
            dic2.Add("total_amount", total_fee);//支付金额
            dic2.Add("product_code", _appSettings.Product_code);
            dic2.Add("body", body);
            dic2.Add("subject", subject);
            dic.Add("biz_content", JsonHelper.SerializeObject(dic2));
            string sign = AlipaySignature.RSASign(dic, privateKeyPem, "utf-8", false, _appSettings.Sign_type);
            sign = HttpUtility.UrlEncode(sign, Encoding.GetEncoding(charset));
            string Parms = string.Empty;
            var testparms = string.Empty;
            foreach (KeyValuePair<String, string> k in dic)
            {
                testparms += k.Key + "=" + k.Value + "&";
                if (Parms == string.Empty)
                {
                    Parms = k.Key + "=" + HttpUtility.UrlEncode(k.Value, Encoding.GetEncoding(charset));
                }
                else
                {
                    Parms += "&" + k.Key + "=" + HttpUtility.UrlEncode(k.Value, Encoding.GetEncoding(charset));
                }
            }

            Parms = Parms + "&sign=" + sign;
            return Parms;

        }

        /// <summary>
        /// 生成支付宝H5支付
        /// author：陶林辉
        /// </summary>
        /// <param name="recharge"></param>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        public AliPayH5View CreateAlipayH5Receipt(RechargeCommodityDto recharge, string order)
        {
            DefaultAopClient client = new DefaultAopClient(_appSettings.Gatewayurl, _appSettings.App_id, _appSettings.PrivateKeyPem, "json", "1.0",
               _appSettings.Sign_type, _appSettings.PublicKey, _appSettings.CharSet, false);
            AliPayH5View aliPayH5View = new AliPayH5View();
            // 组装业务参数model
            AlipayTradePagePayModel model = new AlipayTradePagePayModel
            {
                Body = recharge.Name,//商品描述
                Subject = "乐趣海南充值",//订单名称
                TotalAmount = recharge.Money.ToString(),//付款金额
                OutTradeNo = order, //订单号
                ProductCode = "QUICK_WAP_PAY"
            };
            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            request.SetReturnUrl(_appSettings.Return_url);
            // 设置异步通知接收地址
            request.SetNotifyUrl(_appSettings.Notify_url);
            request.SetBizModel(model);
            aliPayH5View.html = _alipayService.pageExecute(request).Body;
            return aliPayH5View;
        }

        /// <summary>
        /// 记录支付宝订单
        /// author：陶林辉
        /// </summary>
        /// <param name="aliPayOrder"></param>
        /// <returns></returns>
        public async Task<bool> RecordAlipayOrderAsync(AliPayOrderDto aliPayOrder)
        {
            var entity = _mapper.Map<AliPayOrderDto, AliPayOrderEntity>(aliPayOrder);
            _context.AliPayOrder.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据商户订单获取订单详情，
        /// author：陶林辉
        /// </summary>
        /// <param name="val">支付宝商户订单</param>
        /// <returns></returns>
        public async Task<AliPayOrderDto> GetAliPayOrderByIdAsync(string val)
        {
            var entity = await _context.AliPayOrder.Where(m => m.Out_trade_no == val).FirstOrDefaultAsync();
            return _mapper.Map<AliPayOrderEntity, AliPayOrderDto>(entity);
        }

        /// <summary>
        /// 根据商户订单获取订单详情，
        /// author：陶林辉
        /// </summary>
        /// <param name="aliPayOrder">支付宝商户订单</param>
        /// <returns></returns>
        public async Task<bool> UpdateAlipayOrderAsync(AliPayOrderDto aliPayOrder)
        {
            var entity = await _context.AliPayOrder.Where(m => m.Out_trade_no == aliPayOrder.Out_trade_no).FirstOrDefaultAsync();
            entity.trade_no = aliPayOrder.Trade_no;
            entity.Trade_status = aliPayOrder.Trade_status;
            entity.Gmt_payment = aliPayOrder.Gmt_payment;
            entity.updatetime = aliPayOrder.Updatetime;
            entity.Buyer_id = aliPayOrder.Buyer_id;
            entity.Fund_channel = aliPayOrder.Fund_channel;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 记录充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="recharge">object</param>
        /// <returns></returns>
        public async Task<long> RecordRechargeLogAsync(RechargeDto recharge)
        {
            var entity = _mapper.Map<RechargeDto, RechargeEntity>(recharge);
            _context.Recharge.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// 更新充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        public async Task<bool> UpdateRechargeLogAsync(string status, string orderId)
        {
            var entity = await _context.Recharge.Where(m => m.Order_id == orderId).FirstOrDefaultAsync();
            entity.Status = StringExtension.ToInt(status);
            entity.Updatetime = DateTime.Now;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 获取用户充值列表
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<List<RechargeDto>> GetRechargeListAsync(long userId)
        {
            //status 1 待处理 2 已支付 3 支付失败 4 超时
            var list = await _context.Recharge.Where(m => m.UserId == userId && m.Status == 2).ToListAsync();
            var result = _mapper.Map<List<RechargeEntity>, List<RechargeDto>>(list);
            return result;

        }

        /// <summary>
        /// 获取苹果内购商品信息
        /// author：陶林辉
        /// </summary>
        /// <param name="id">商品id</param>
        /// <returns></returns>
        public async Task<AppleProductDto> GetAppleProductAsync(int id)
        {
            var entity = await _context.RechargeCommodity.FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<RechargeCommodityEntity, AppleProductDto>(entity);
        }

        /// <summary>
        /// 验证苹果收据
        /// author：陶林辉
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        public bool VerifyReceipt(ReceiptDto receipt)
        {
            string strJosn = string.Format("{{\"receipt-data\":\"{0}\"}}", receipt.Receipt);
            //string strResult = CreatePostHttpResponse(strJosn,true);
            string strResult = _sys.PostJsonData(_applepay.Url, strJosn, Encoding.UTF8);
            JObject obj = JObject.Parse(strResult);
            // 判断是否购买成功  
            if (obj["status"].ToString() == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 记录苹果充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="appleReceiptDto"></param>
        /// <returns></returns>
        public async Task<bool> RecordAppleOrderAsync(AppleReceiptDto appleReceiptDto)
        {
            var entity = _mapper.Map<AppleReceiptDto, AppleReceiptEntity>(appleReceiptDto);
            _context.AppleReceipt.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 更新苹果订单
        /// author：陶林辉
        /// </summary>
        /// <param name="appleReceiptDto"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAppleOrderAsync(AppleReceiptDto appleReceiptDto)
        {
            var entity = await _context.AppleReceipt.FirstOrDefaultAsync(m => m.Order_id == appleReceiptDto.Order_id);
            entity.Receipt = appleReceiptDto.Receipt;
            entity.Updatetime = appleReceiptDto.Updatetime;
            return await _context.SaveChangesAsync() > 0; ;
        }

        /// <summary>
        /// 获取苹果内购商品信息
        /// author：陶林辉
        /// </summary>
        /// <param name="appleId">苹果商品ID</param>
        /// <returns></returns>
        public async Task<RechargeCommodityDto> GetAppleProductAsync(string appleId)
        {
            var entity = await _context.RechargeCommodity.FirstOrDefaultAsync(m => m.Apple_id == appleId);
            return _mapper.Map<RechargeCommodityEntity, RechargeCommodityDto>(entity);
        }

        /// <summary>
        /// 判断订单号是否存在
        /// author：陶林辉
        /// </summary>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        public async Task<bool> JudgeOrderIsNoAsync(string order)
        {
            var count = await _context.Recharge.CountAsync(m => m.Order_id == order);
            return count > 0;
        }

        /// <summary>
        /// 判断充值订单是否充值成功
        /// author:陶林辉
        /// </summary>
        /// <param name="order">商户订单号</param>
        /// <returns></returns>
        public async Task<bool> JuderOrderIsSuccess(string order)
        {
            var count = await _context.Recharge.CountAsync(m => m.Order_id == order && m.Status == 2);
            return count > 0;
        }

        /// <summary>
        /// 判断充值订单是否在充值中
        ///  author:陶林辉
        /// </summary>
        /// <returns>The order isimpleme.</returns>
        /// <param name="order">订单号</param>
        public async Task<bool> JuderOrderIsimplemeAsync(string order)
        {
            var count = await _context.Recharge.CountAsync(m => m.Order_id == order && m.Status == 1);
            return count > 0;
        }

        /// <summary>
        /// 海峡竞技生成支付宝订单
        /// author:白尚德
        /// </summary>
        /// <param name="recharge">商品对象</param>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        public string CreateAlipaySign(RechargeCommodityDto recharge, string order)
        {

            string privateKeyPem = _straitAliPay.PrivateKeyPem;//商户私钥
            string out_trade_no = order;//订单号
            double total_fee = double.Parse(recharge.Money.ToString());//交易金额
            string app_id = _straitAliPay.App_id;//app支付，支付宝中该应用的ID
            string charset = "utf-8";//utf-8
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string notify_url = _straitAliPay.Notify_url;//回调地址
            string body = recharge.Name;
            string subject = "海峡竞技充值";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            dic.Add("app_id", app_id);
            dic.Add("method", "alipay.trade.app.pay");
            dic.Add("version", "1.0");
            dic.Add("charset", charset);
            dic.Add("notify_url", notify_url);
            dic.Add("sign_type", _straitAliPay.Sign_type);
            dic.Add("timestamp", timestamp);
            dic2.Add("out_trade_no", out_trade_no);//商户订单号
            dic2.Add("total_amount", total_fee);//支付金额
            dic2.Add("product_code", _straitAliPay.Product_code);
            dic2.Add("body", body);
            dic2.Add("subject", subject);
            dic.Add("biz_content", JsonHelper.SerializeObject(dic2));
            string sign = AlipaySignature.RSASign(dic, privateKeyPem, "utf-8", false, _straitAliPay.Sign_type);
            sign = HttpUtility.UrlEncode(sign, Encoding.GetEncoding(charset));
            string Parms = string.Empty;
            var testparms = string.Empty;
            foreach (KeyValuePair<String, string> k in dic)
            {
                testparms += k.Key + "=" + k.Value + "&";
                if (Parms == string.Empty)
                {
                    Parms = k.Key + "=" + HttpUtility.UrlEncode(k.Value, Encoding.GetEncoding(charset));
                }
                else
                {
                    Parms += "&" + k.Key + "=" + HttpUtility.UrlEncode(k.Value, Encoding.GetEncoding(charset));
                }
            }

            Parms = Parms + "&sign=" + sign;
            return Parms;

        }

        /// <summary>
        /// 获取充值类型
        /// author:白尚德
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public async Task<RechargeTypeDto> GetRechargeAsync(string orderid)
        {
            try
            {
                var result = await _context.Recharge.Where(m => m.Status == 2 && m.Order_id == orderid).FirstOrDefaultAsync();
                return _mapper.Map<RechargeEntity, RechargeTypeDto>(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 获取充值类型
        /// author:白尚德
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public async Task<GiveCurrencyLogDto> GetSignAsync(string orderid)
        {
            try
            {
                var result =await _context.giveCurrencyLog.Where(m => m.Order == orderid).FirstOrDefaultAsync();

                return _mapper.Map<GiveCurrencyLogEntity, GiveCurrencyLogDto>(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //end
    }
}
