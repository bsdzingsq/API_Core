using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class UserLoginEntity
    {
        [Key]
        public long Userid { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public int Status { get; set; }

        public int Is_first { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public string Token { get; set; }

        public int Salt { get; set; }
    }
}
