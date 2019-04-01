using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.ViewModel.Recharge;

namespace ZsqApp.Core.Interfaces.Recharge
{

    //**************操作记录******************
    //创建时间：2018.02.29
    //作者：陶林辉
    //内容描述：充值接口定义
    //***************************************
    public interface IRecharge
    {
        /// <summary>
        /// 获取充值商品列表
        /// author:陶林辉
        /// </summary>
        /// <param name="type">商品类型 0普通，1ios审核商品</param>
        /// <returns></returns>
        Task<List<RechargeCommodityDto>> GetRechargeCommodityAsync(int type);

        /// <summary>
        /// 根据Id获取单个充值商品
        /// author:陶林辉
        /// </summary>
        /// <param name="type">商品id</param>
        /// <returns></returns>
        Task<RechargeCommodityDto> GetRechargeCommodityByIdAsync(int id);

        /// <summary>
        /// 生成支付宝订单
        /// author:陶林辉
        /// </summary>
        /// <param name="recharge">商品对象</param>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        string CreateAlipayReceipt(RechargeCommodityDto recharge, string order);

        /// <summary>
        /// 记录支付宝订单
        /// author：陶林辉
        /// </summary>
        /// <param name="aliPayOrder"></param>
        /// <returns></returns>
        Task<bool> RecordAlipayOrderAsync(AliPayOrderDto aliPayOrder);

        /// <summary>
        /// 更新支付宝订单
        /// author：陶林辉
        /// </summary>
        /// <param name="aliPayOrder"></param>
        /// <returns></returns>
        Task<bool> UpdateAlipayOrderAsync(AliPayOrderDto aliPayOrder);

        /// <summary>
        /// 根据商户订单获取订单详情，
        /// author：陶林辉
        /// </summary>
        /// <param name="val">支付宝商户订单</param>
        /// <returns></returns>
        Task<AliPayOrderDto> GetAliPayOrderByIdAsync(string val);

        /// <summary>
        /// 记录充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="recharge">object</param>
        /// <returns></returns>
        Task<long> RecordRechargeLogAsync(RechargeDto recharge);

        /// <summary>
        /// 更新充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        Task<bool> UpdateRechargeLogAsync(string status, string orderId);

        /// <summary>
        /// 获取用户充值列表
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        Task<List<RechargeDto>> GetRechargeListAsync(long userId);

        /// <summary>
        /// 获取苹果内购商品信息
        /// author：陶林辉
        /// </summary>
        /// <param name="id">商品id</param>
        /// <returns></returns>
        Task<AppleProductDto> GetAppleProductAsync(int id);

        /// <summary>
        /// 验证苹果收据
        /// author：陶林辉
        /// </summary>
        /// <param name="receipt"></param>
        /// <returns></returns>
        bool VerifyReceipt(ReceiptDto receipt);

        /// <summary>
        /// 记录苹果充值记录
        /// author：陶林辉
        /// </summary>
        /// <param name="appleReceiptDto"></param>
        /// <returns></returns>
        Task<bool> RecordAppleOrderAsync(AppleReceiptDto appleReceiptDto);

        /// <summary>
        /// 更新苹果订单
        /// author：陶林辉
        /// </summary>
        /// <param name="appleReceiptDto"></param>
        /// <returns></returns>
        Task<bool> UpdateAppleOrderAsync(AppleReceiptDto appleReceiptDto);

        /// <summary>
        /// 获取苹果内购商品信息
        /// author：陶林辉
        /// </summary>
        /// <param name="appleId">苹果商品ID</param>
        /// <returns></returns>
        Task<RechargeCommodityDto> GetAppleProductAsync(string appleId);

        /// <summary>
        /// 判断订单号是否存在
        /// author：陶林辉
        /// </summary>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        Task<bool> JudgeOrderIsNoAsync(string order);

        /// <summary>
        /// 生成支付宝H5支付
        /// author：陶林辉
        /// </summary>
        /// <param name="recharge"></param>
        /// <param name="order">订单号</param>
        /// <returns></returns>
        AliPayH5View CreateAlipayH5Receipt(RechargeCommodityDto recharge, string order);

        /// <summary>
        /// 判断充值订单是否充值成功
        /// author:陶林辉
        /// </summary>
        /// <param name="order">商户订单号</param>
        /// <returns></returns>
        Task<bool> JuderOrderIsSuccess(string order);

        /// <summary>
        /// 判断充值订单是否在充值中
        ///  author:陶林辉
        /// </summary>
        /// <returns>The order isimpleme.</returns>
        /// <param name="order">订单号</param>
        Task<bool> JuderOrderIsimplemeAsync(string order);

        /// <summary>
        /// 海峡竞技生成支付宝订单
        /// author:白尚德
        /// <param name="recharge"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        string CreateAlipaySign(RechargeCommodityDto recharge, string order);
    }
}
