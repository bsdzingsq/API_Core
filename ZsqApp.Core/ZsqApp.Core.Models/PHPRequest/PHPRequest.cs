using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Infrastructure.Utilities;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class PHPRequest
    {
        public PHPRequest()
        {
            this.RequestId = System.Guid.NewGuid().ToString("N");
            this.Language = "cn";
            this.Timestamp = TimeHelper.GetCurrentTimeUnix().ToString();
            this.Custom = "";
            this.Nonce = System.Guid.NewGuid().ToString("N").Substring(0, 8);
            this.SignType = "SHA_512";
            this.Encode = true;

        }
        public string AppKey { get; set; }

        public string RequestId { get; set; }

        public string Timestamp { get; set; }

        public string Custom { get; set; }

        public string Nonce { get; set; }

        public string Language { get; set; }

        public string Sign { get; set; }

        public string SignType { get; set; }

        public bool Encode { get; set; }

        public string Data { get; set; }

        public pagerequest Page { get; set; }
    }
    public class pagerequest
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
