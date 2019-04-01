using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class RechargeRequest
    {
        public string fuserId { get; set; }

        public string forderId { get; set; }

        public double amount { get; set; }

        public string rateKey { get; set; }
    }
}
