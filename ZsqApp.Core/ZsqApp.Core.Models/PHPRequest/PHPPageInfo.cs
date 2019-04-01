using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
   public class PHPPageInfo
    {
        public int Count { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageTotal { get; set; }

        public int Totaltotal { get; set; }
    }
}
