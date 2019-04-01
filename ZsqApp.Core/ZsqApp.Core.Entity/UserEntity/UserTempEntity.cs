using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class UserTempEntity
    {
        [Key]
        public long user_id { get; set; }

        public string phone { get; set; }

        public string nick_name { get; set; }

        public string real_name { get; set; }

        public string id_card { get; set; }

        public string channel_id { get; set; }

        public string version { get; set; }

        public string os_type { get; set; }

        public string device_code { get; set; }

        public string os_version { get; set; }

        public int status { get; set; }

        public DateTime create_time { get; set; }

        public DateTime update_time { get; set; }
        public string password { get; set; }
        public int salt { get; set; }
    }
}
