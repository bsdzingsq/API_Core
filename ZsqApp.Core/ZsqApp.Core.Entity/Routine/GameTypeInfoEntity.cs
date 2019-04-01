using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class GameTypeInfoEntity
    {
        [Key]
        public long id { get; set; }
        public string type_name { get; set; }
        public DateTime createtime { get; set; }
        public DateTime updatetime { get; set; }
    }
}
