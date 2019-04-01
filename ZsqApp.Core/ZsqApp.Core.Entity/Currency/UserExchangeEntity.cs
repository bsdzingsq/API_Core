using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Currency
{
    public class UserExchangeEntity
    {
        [Key]
        public long Id { get; set; }
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
