using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Recharge
{
    public class RechargeCommodityEntity
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Money { get; set; }

        public decimal Amount { get; set; }

        public decimal Give { get; set; }

        public int Is_status { get; set; }

        public int Rank { get; set; }

        public string Icon_url { get; set; }

        public int Type { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public string Product_id { get; set; }

        public string Apple_id { get; set; }
    }
}
