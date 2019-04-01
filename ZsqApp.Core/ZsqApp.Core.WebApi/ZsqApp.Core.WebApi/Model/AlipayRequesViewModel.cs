using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZsqApp.Core.WebApi.Model
{
    public class AlipayRequesViewModel
    {
        /// <summary>
        /// 通知的发送时间。格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        public DateTime notify_time { get; set; }

        /// <summary>
        /// 通知类型
        /// </summary>
        public string notify_type { get; set; }

        /// <summary>
        /// 通知校验ID
        /// </summary>
        public string notify_id { get; set; }

        /// <summary>
        /// 支付宝分配给开发者的应用Id
        /// </summary>
        public string app_id { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public string charset { get; set; }

        /// <summary>
        /// 接口版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        public string sign_type { get; set; }

        /// <summary>
        /// 签名s
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        public string trade_no { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 买家支付宝账号
        /// </summary>
        public string buyer_logon_id { get; set; }

        /// <summary>
        /// 卖家支付宝账号 
        /// </summary>
        public string seller_email { get; set; }

        /// <summary>
        /// 交易状态
        /// WAIT_BUYER_PAY	交易创建，等待买家付款
        /// TRADE_CLOSED	未付款交易超时关闭，或支付完成后全额退款
        /// TRADE_SUCCESS	交易支付成功
        /// TRADE_FINISHED	交易结束，不可退款
        /// </summary>
        public string trade_status { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public string total_amount { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public string receipt_amount { get; set; }

        /// <summary>
        /// 支付金额信息
        /// </summary>
        public Fund fund_bill_list { get; set; }
    }

    public class Fund
    {
        /// <summary>
        /// COUPON	支付宝红包
        /// ALIPAYACCOUNT	支付宝余额
        /// POINT 集分宝
        /// DISCOUNT 折扣券
        /// PCARD 预付卡
        /// FINANCEACCOUNT 余额宝
        /// MCARD 商家储值卡
        /// MDISCOUNT 商户优惠券
        /// MCOUPON 商户红包
        /// PCREDIT 蚂蚁花呗
        /// </summary>
        public string fundChannel { get; set; }

        public string amount { get; set; }
    }
}
