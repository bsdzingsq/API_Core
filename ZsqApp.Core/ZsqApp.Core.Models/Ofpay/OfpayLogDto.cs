using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Ofpay
{
    public class OfpayLogDto
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 欧非商品id
        /// </summary>
        public long OfPay_Id { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string Order_Id { get; set; }

        /// <summary>
        /// 欧飞订单号
        /// </summary>
        public string OfPay_Order_Id { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 充值手机号码
        /// </summary>
        public string Phone { get; set; }

        public decimal Ordercash { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
