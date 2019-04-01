using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Currency
{
    public class GiveCurrencyLogEntity
    {
        [Key]
        public long Id { get; set; }

        public string Key { get; set; }

        public long UserId { get; set; }

        public string Order { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
