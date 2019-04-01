using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
  public  class ExchangeDto
    {
        public string FuserId { get; set; }
        public string ForderId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
