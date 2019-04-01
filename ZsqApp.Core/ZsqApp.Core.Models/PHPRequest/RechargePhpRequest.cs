using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
     public class RechargePhpRequest
    {
        public string fuserId { get; set; }

        public string forderId { get; set; }

        public double amount { get; set; }

        public string description { get; set; }

        public string rateKey { get; set; }
    }
}
