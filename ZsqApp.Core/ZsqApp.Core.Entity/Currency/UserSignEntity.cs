using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Currency
{
    public class UserSignEntity
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }
        
        public int Number { get; set; }

        public decimal Amount { get; set; }

        public decimal Multiple { get; set; }

        public DateTime Createtime { get; set; }


    }
}
