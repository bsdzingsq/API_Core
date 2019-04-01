using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Ofpay
{
    public class OfpayLogEntity
    {
        [Key]
        public long Id { get; set; }

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
        public string Order_id { get; set; }

        /// <summary>
        /// 欧飞订单号
        /// </summary>
        public string Ofpay_Order_Id { get; set; }

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
        public DateTime Createtime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime Updatetime { get; set; }
    }
}
