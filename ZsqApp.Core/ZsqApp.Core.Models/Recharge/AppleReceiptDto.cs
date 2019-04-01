using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Recharge
{
    public class AppleReceiptDto
    {

        public long Item_id { get; set; }

        public string Apple_id { get; set; }

        public string Order_id { get; set; }

        public string Receipt { get; set; }

        public long Userid { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
