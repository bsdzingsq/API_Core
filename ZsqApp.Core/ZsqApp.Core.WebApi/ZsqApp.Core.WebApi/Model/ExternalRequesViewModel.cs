using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZsqApp.Core.WebApi.Model
{
    public class ExternalRequesViewModel
    {
        public string AppKey { get; set; }

        public string Sign { get; set; }

        public string Stimestamp { get; set; }

        public dynamic Data { get; set; }

    }
}
