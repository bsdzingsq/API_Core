using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Currency
{
    public class GiveCurrencyLogDto
    {
        public string Key { get; set; }

        public long UserId { get; set; }

        public string Order { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
