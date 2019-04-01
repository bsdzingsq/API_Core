using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class MatchType
    {
        public long sportType { get; set; }

        public string displayName { get; set; }

        public string iconUrl { get; set; }

        public int matchCount { get; set; }
    }
    public class MatchTypeList
    {
        public List<MatchType> data { get; set; }

        public int Count { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageTotal { get; set; }
        public int Totaltotal { get; set; }
    }
}
