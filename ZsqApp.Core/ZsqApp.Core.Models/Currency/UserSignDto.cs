using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Currency
{
    public class UserSignDto
    {
        public long UserId { get; set; }

        public int Number { get; set; }

        public decimal Amount { get; set; }

        public decimal Multiple { get; set; }

        public DateTime Createtime { get; set; }
    }
}
