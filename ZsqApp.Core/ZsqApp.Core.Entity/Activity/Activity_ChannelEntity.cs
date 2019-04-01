using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.Entity.Activity
{
    public class Activity_ChannelEntity
    {
          
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string Channel { get; set; }
        public int Activity_Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
