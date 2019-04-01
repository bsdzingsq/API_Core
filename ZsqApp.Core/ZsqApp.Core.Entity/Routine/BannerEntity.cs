using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class BannerEntity
    {
        [Key]
        public long Id { get; set; }

        public int Banner_type { get; set; }

        public string Pic_url { get; set; }

        public string Target { get; set; }

        public int Type_url { get; set; }

        public string Description { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public int Rank { get; set; }

        public int Is_status { get; set; }

        public DateTime? Start_time { get; set; }

        public DateTime? Over_time { get; set; }
    }
}
