using System;

namespace ZsqApp.Core.Models.Recharge
{
    public class AliPayOrderDto
    {
        public long Commodity_id { get; set; }

        public string Out_trade_no { get; set; }

        public decimal Total_amount { get; set; }

        public long Userid { get; set; }

        public string Trade_no { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public DateTime Gmt_payment { get; set; }

        public string Trade_status { get; set; }

        public string Buyer_id { get; set; }


        public string Fund_channel { get; set; }
    }
}
