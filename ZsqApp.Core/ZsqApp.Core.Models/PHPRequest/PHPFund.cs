using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class PHPFund
    {
        public string OrderId { get; set; }

        public int Type { get; set; }

        public string Time { get; set; }

        public double Amount { get; set; }

        public double Balance { get; set; }

        public string Desc { get; set; }

    }

    public class PHPFundList
    {
        public List<PHPFund> Data { get; set; }

        public PHPPageInfo Page { get; set; }
    }
}
