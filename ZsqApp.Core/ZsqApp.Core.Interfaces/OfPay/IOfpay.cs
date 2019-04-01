using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Ofpay;
using ZsqApp.Core.ViewModel.OfPay;

namespace ZsqApp.Core.Interfaces.OfPay
{
    //**************操作记录******************
    //创建时间：2018.03.29
    //作者：白尚德
    //内容描述：应用相关内部接口
    //***************************************
    public interface IOfpay
    {

        /// <summary>
        /// 获取欧飞商品
        /// author:白尚德
        /// </summary>
        /// <returns></returns>
        Task<List<OfpayDto>> GetOfOayListAsync();

        /// <summary>
        /// 获取用户手机号码及归属地
        /// author:白尚德
        /// </summary>
        /// <param name="mobilenum"></param>
        /// <returns></returns>
        MobInfoViewModel MobInfo(string mobilenum);

        /// <summary>
        /// 根据商品id获取信息
        /// author:白尚德
        /// </summary>
        /// <returns></returns>
        Task<OfpayDto> GetOfOayByIdAsync(long id);

        /// <summary>
        /// 当前手机号码是否能充值当前商品
        /// author:白尚德
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        bool GetOfOayIsNo(string phone, long price);

        /// <summary>
        /// 记录话费充值订单
        /// </summary>
        /// <param name="ofpayLogDto"></param>
        /// <returns></returns>
        Task<bool> OfpayLogAsync(OfpayLogDto ofpayLogDto);

        /// <summary>
        /// 话费充值
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="cardnum">面额</param>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        Task<bool> PrepaidRefillAsync(string phone, string cardnum, string orderId);

        /// <summary>
        /// 更新欧非充值订单
        /// author:陶林辉
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="orderId">订单号</param>
        /// <param name="ordercash">欧飞价格</param>
        /// <param name="ofPayOrderId">欧飞订单号</param>
        /// <returns></returns>
        Task<bool> UpdateOfpayLogAsync(int status, string orderId, decimal ordercash, string ofPayOrderId);
    }
}
