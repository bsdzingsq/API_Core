using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class StraitAppVersionEntity
    {
        [Key]
        public long Id { get; set; }
        public string Version { get; set; }
        public long Version_code { get; set; }
        public int Platform { get; set; }
        public int Present_status { get; set; }
        public string Renewal { get; set; }
        public string Remarks { get; set; }
        public string Download_url { get; set; }
        public string Package_size { get; set; }
        public int Active_remind { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
    }
}
