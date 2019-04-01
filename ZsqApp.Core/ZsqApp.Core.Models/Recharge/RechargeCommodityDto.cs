using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Recharge
{
    public class RechargeCommodityDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public decimal Money { get; set; }

        public decimal Amount { get; set; }

        public decimal Give { get; set; }

        public string Icon_url { get; set; }

    }
}
