using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class PHPResponse
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string AppKey { get; set; }

        public string RequestId { get; set; }

        public string Custom { get; set; }

        public string Timestamp { get; set; }

        public string Nonce { get; set; }

        public string Sign { get; set; }

        public string SignType { get; set; }

        public string Data { get; set; }

        public page Page { get; set; }
    }
    public class page
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageTotal { get; set; }

        public int Count { get; set; }

        public int Totaltotal { get; set; }
    }
}
