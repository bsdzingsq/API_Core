using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Currency
{
 
    public class FiveOneRightEntity
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
