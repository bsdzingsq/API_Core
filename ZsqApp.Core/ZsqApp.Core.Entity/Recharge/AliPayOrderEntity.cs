using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Recharge
{
    public class AliPayOrderEntity
    {
        [Key]
        public long Id { get; set; }

        public long Commodity_id { get; set; }

        public string Out_trade_no { get; set; }

        public decimal Total_amount { get; set; }

        public long userid { get; set; }

        public string trade_no { get; set; }

        public DateTime createtime { get; set; }

        public DateTime updatetime { get; set; }

        public DateTime Gmt_payment { get; set; }

        public string Trade_status { get; set; }
  
        public string Buyer_id { get; set; }

        public string Fund_channel { get; set; }
    }
}
