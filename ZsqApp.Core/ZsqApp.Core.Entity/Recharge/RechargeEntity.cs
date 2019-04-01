using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Recharge
{
    public class RechargeEntity
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public decimal Amount { get; set; }

        public string Pay_type { get; set; }

        /// <summary>
        /// 1 待处理 2 已支付 3 支付失败 4 超时
        /// </summary>
        public int Status { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public string Order_id { get; set; }
    }
}
