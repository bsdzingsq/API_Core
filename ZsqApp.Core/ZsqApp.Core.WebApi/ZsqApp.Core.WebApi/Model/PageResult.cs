using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZsqApp.Core.WebApi.Model
{
    public class PageResult
    {
        public int Count { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int PageTotal { get; set; }

        public int Totaltotal { get; set; }
    }
}
