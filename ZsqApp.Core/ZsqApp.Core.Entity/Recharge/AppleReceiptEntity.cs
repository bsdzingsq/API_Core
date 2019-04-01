using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Recharge
{
    public class AppleReceiptEntity
    {
        [Key]
        public int Id { get; set; }

        public long Item_id { get; set; }

        public string Apple_id { get; set; }

        public string Order_id { get; set; }

        public string Receipt { get; set; }

        public long Userid { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
