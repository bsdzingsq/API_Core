using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.Entity.Activity
{
    public class Activity_TypeEntity
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(155)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
