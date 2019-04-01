using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class NewUserChannelEntity
    {
        [Key]
        public int Id { get; set; }

        public long UserId { get; set; }

        public string NewChannelId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
