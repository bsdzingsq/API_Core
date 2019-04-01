using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Recharge
{
    public class ReceiptDto
    {
        public string Id { get; set; }

        public string OrderId { get; set; }

        public string AppleId { get; set; }

        public string Receipt { get; set; }
    }
}
