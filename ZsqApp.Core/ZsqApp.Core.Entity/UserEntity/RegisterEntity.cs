using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{
    public class RegisterEntity
    {
        [Key]
        public long Userid { get; set; }

        public string Channel { get; set; }

        public string Platform { get; set; }

        public string Phone { get; set; }

        public string Gps { get; set; }

        public string App_version { get; set; }

        public string Device_code { get; set; }

        public string Os_type { get; set; }

        public string Os_version { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
