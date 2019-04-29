using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Recharge
{
    public class RechargeTypeDto
    {
        public long UserId { get; set; }
        public string Order_id { get; set; }
        public string Pay_type { get; set; }
    }
}
