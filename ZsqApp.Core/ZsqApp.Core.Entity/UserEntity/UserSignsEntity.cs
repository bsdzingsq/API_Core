using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class UserSignsEntity
    {
        [Key]
        public int userSignid { get; set; }

        public int userId { get; set; }
        public int userSignNumber { get; set; }
        public DateTime signTime { get; set; }

        public decimal amount { get; set; }

        public decimal Multiple { get; set; }

        public string channelId { get; set; }

    }
}
