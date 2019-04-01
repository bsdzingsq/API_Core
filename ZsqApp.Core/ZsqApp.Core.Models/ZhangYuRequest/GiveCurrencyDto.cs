using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class GiveCurrencyDto
    {
        public string fuserId { get; set; }

        public string forderId { get; set; }

        public string key { get; set; }

        public double amount { get; set; }
    }
}
