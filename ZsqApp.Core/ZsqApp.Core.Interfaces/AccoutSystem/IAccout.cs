using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.PHPRequest;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;

namespace ZsqApp.Core.Interfaces.AccoutSystem
{
    //**************操作记录******************
    //创建时间：2018.08.24
    //作者：白尚德
    //内容描述：php接口定义
    //***************************************
    public interface IAccout
    {
        /// <summary>
        /// 账户充值
        /// author：白尚德  
        /// </summary>
        /// <param name="fuserId">用户id</param>
        /// <param name="forderId">订单号</param>
        /// <param name="amount">充值虚拟币金额</param>
        /// <param name="description">描述</param>
        /// <param name="key">充值key</param>
        /// <returns></returns>
        bool Recharge_php(long fuserId, string forderId, double amount, string description, string key);

        /// <summary>
        /// 商品兑换
        /// author:白尚德
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        Task<bool> Exchange_php(ExchangeDto exchange);

        /// <summary>
        /// 账户退款
        /// author:白尚德
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        Task<bool> Refund_php(RefundDto refund);

        /// <summary>
        /// 查询用户流水
        /// author：白尚德  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        FundList AcquireFund_php(long userId, int type, int pageIndex, int pageSize);

        /// <summary>
        /// 查询用户余额
        /// author：白尚德  
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>余额，可兑出</returns>
        UserBalanceDto AcquireBalance_php(long userId);

        /// <summary>
        /// 根据兑出订单号查询订单状态
        /// author:白尚德
        /// </summary>
        /// <param name="forderId">第三方订单号</param>
        /// <returns></returns>
        CostStatusResult CostStatus_php(string forderId);

        /// <summary>
        /// 赠送货币
        /// author：白尚德 
        /// </summary>
        /// <returns></returns>
        Task<bool> GiveCurrencyAsync_php(GiveCurrencyDto giveCurrency);

        /// <summary>
        ///  海峡竞技消费调用章鱼接口2.1
        /// author:白尚德
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        Task<bool> Consume(ConsumeDto consume);

        /// <summary>
        /// 海峡竞技消费PHP
        /// </summary>
        /// <param name="consume"></param>
        /// <returns></returns>
        Task<bool> Consume_php(ConsumeDto consume);

        /// <summary>
        /// 海峡竞技从章鱼获取订单状态
        /// </summary>
        /// <param name="forderId"></param>
        /// <returns></returns>
        CostStatusResult CostOrderStatus(string forderId);

        /// <summary>
        /// 海峡竞技从php获取订单状态
        /// </summary>
        /// <param name="forderId"></param>
        /// <returns></returns>
        CostStatusResult CostOrderStatus_php(string forderId);

        /// <summary>
        /// 从php获取公众号充值订单
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        List<orderInfoList> GetRechargeTypes(string strJson);
    }
}
