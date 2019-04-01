using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZsqApp.Core.WebApi.Model
{
    public class WeCharRequest
    {

        public string userOpenId { get; set; }

        public string sessionToken { get; set; }

        public string itemId { get; set; }

    }

    public class WeCharRequestList
    {
        public WeCharRequest data { get; set; }

    }
}
