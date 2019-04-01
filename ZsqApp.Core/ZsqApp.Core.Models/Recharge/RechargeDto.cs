using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Recharge
{
    public class RechargeDto
    {

        public long UserId { get; set; }

        public decimal Amount { get; set; }

        public int Pay_type { get; set; }

        /// <summary>
        /// 1 待处理 2 已支付 3 支付失败 4 超时
        /// </summary>
        public int Status { get; set; }

        public DateTime createtime { get; set; }

        public DateTime updatetime { get; set; }
        public string Order_id { get; set; }
    }
}
