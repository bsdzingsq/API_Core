using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.HomePage
{
    public class StraitHomePageEntity
    {
        [Key]
        public long Id { get; set; }
        public int HomePage_Type { get; set; }
        public string ReadImg { get; set; }
        public string ReadTitle { get; set; }
        public decimal ReadPrice { get; set; }
        public decimal ReadMonery { get; set; }
        public int Status { get; set; }
        public int Short { get; set; }
        public string Description { get; set; }
        public string Link_Url { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
