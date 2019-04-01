using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class UserInfoEntity
    {
        [Key]
        public long Userid { get; set; }

        public string Nick_name { get; set; }

        public string Real_name { get; set; }

        public string Id_card { get; set; }

        public string Head { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
