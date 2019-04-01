using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.User
{
    public class FeedbackDto
    {
        public long Id { get; set; }

        public long Userid { get; set; }

        public string App_version { get; set; }

        public string Content { get; set; }

        public int Respond { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }
    }
}
