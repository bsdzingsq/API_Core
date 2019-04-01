using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class Fund
    {

        public string OrderId { get; set; }

        public int Type { get; set; }

        public string Time { get; set; }

        public double Amount { get; set; }

        public double Balance { get; set; }

        public string Desc { get; set; }

    }

    public class FundList
    {
        public List<Fund> data { get; set; }

        public PageInfo Page { get; set; }
    }
}
