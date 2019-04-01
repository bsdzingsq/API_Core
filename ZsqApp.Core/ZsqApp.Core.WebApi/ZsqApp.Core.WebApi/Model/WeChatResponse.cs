using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZsqApp.Core.WebApi.Model
{
    public class WeChatResponse
    {
        public string Code { get; set; }

        public string Message { get; set; }
        public data Data { get; set; }

    }
    public class data
    {
        public string appid { get; set; }

        public string noncestr { get; set; }

        public string package { get; set; }

        public string partnerid { get; set; }

        public string prepayid { get; set; }
        public string sign { get; set; }
        public string timestamp { get; set; }
    }
}
