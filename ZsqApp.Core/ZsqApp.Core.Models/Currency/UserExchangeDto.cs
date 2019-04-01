using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Currency
{
   public class UserExchangeDto
    {
        public long UserId { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateTime { get; set; }
        public int Status { get; set; }
    }
}
