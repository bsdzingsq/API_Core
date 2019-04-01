using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class UserSynchEntity
    {
        [Key]
        public int userSynchId { get; set; }

        public long userId { get; set; }

        public string phone { get; set; }

        public string nickName { get; set; }

        public string realName { get; set; }

        public string idCard { get; set; }

        public string channelId { get; set; }

        public string version { get; set; }

        public string osType { get; set; }

        public string deviceCode { get; set; }

        public string osVersion { get; set; }

        public int status { get; set; }

        public DateTime createTime { get; set; }

        public DateTime updateTime { get; set; }

        public string firstchannelId { get; set; }

        public string pwd { get; set; }

        public int salt { get; set; }
    }
}
