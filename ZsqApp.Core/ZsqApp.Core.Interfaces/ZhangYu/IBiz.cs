using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;

namespace ZsqApp.Core.Interfaces.ZhangYu
{
    //**************操作记录******************
    //创建时间：
    //作者：陶林辉
    //内容描述：章鱼接口定义
    //***************************************
    public interface IBiz
    {
        /// <summary>
        /// 查询用户余额
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>余额，可兑出</returns>
        UserBalanceDto AcquireBalance(long userId);

        /// <summary>
        /// 查询用户流水
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        FundList AcquireFund(long userId, int type, int pageIndex, int pageSize);


        /// <summary>
        /// 账户充值
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="orderId">订单号</param>
        /// <param name="amount">充值虚拟币金额</param>
        /// <param name="key">充值key</param>
        /// <returns></returns>
        bool Recharge(long userId, string orderId, double amount, string key);


        /// <summary>
        /// 查询用户竞猜列表
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        OrderList AcquireOrder(long userId, int type, int pageIndex, int pageSize);

        /// <summary>
        /// 竞猜投注详情
        /// author：陶林辉 
        /// </summary>
        /// <param name="forderId">第三方订单 Id</param>
        /// <returns></returns>
        Order AcquireOrderDetail(string forderId);

        /// <summary>
        /// 获取竞猜游戏列表
        /// author：刘嘉辉 
        /// </summary>
        /// <returns></returns>
        MatchTypeList AcquireGuessMatch();

        /// <summary>
        /// 赠送货币
        /// author：林辉 
        /// </summary>
        /// <returns></returns>
        Task<bool> GiveCurrencyAsync(GiveCurrencyDto giveCurrency);

        /// <summary>
        /// 商品兑换
        /// author:林辉
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        Task<bool> Exchange(ExchangeDto exchange);

        /// <summary>
        /// 账户退款
        /// author:林辉
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        Task<bool> Refund(RefundDto refund);

        /// <summary>
        /// 获取游戏地址
        /// author:林辉
        /// </summary>
        /// <param name="gameKey">游戏key</param>
        /// <param name="userId">用户id</param>
        /// <param name="version">APP版本</param>
        /// <param name="osType">来源系统</param>
        /// <param name="deviceCode">设备型号</param>
        /// <param name="nickName">用户昵称</param>
        /// <returns></returns>
        GameUrlResponse GameUrl(string gameKey, long userId, string version, int osType, string deviceCode, string nickName);

        /// <summary>
        /// 章鱼1.4用户认证
        /// author:林辉
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userOpenId">userOpenId</param>
        /// <returns></returns>
        string IsNoLogin(string token, string userOpenId);

        /// <summary>
        /// 根据兑出订单号查询订单状态
        /// author:林辉
        /// </summary>
        /// <param name="forderId">第三方订单号</param>
        /// <returns></returns>
        CostStatusResult CostStatus(string forderId);
    }
}
