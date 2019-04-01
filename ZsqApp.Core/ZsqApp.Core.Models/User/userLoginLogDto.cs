using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.User
{
    public class userLoginLogDto
    {
        public long Id { get; set; }

        public long Userid { get; set; }

        public string Phone { get; set; }

        public string Device_code { get; set; }

        public string Os_type { get; set; }

        public string Os_version { get; set; }

        public string Gps { get; set; }

        public string App_version { get; set; }

        public DateTime Createtime { get; set; }
    }
}
