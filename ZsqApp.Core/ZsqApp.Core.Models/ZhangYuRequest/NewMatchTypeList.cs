using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class NewMatchTypeList
    {
        public string code { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
        public MatchList data { get; set; }

        public class MatchList
        {
            public List<MatchTypeData> list { get; set; }
        }
        public class MatchTypeData
        {
            public string description { get; set; }
            public string displayName { get; set; }
            public string iconUrl { get; set; }
            public int matchCount { get; set; }
            public long sportType { get; set; }
        }
    }
}
