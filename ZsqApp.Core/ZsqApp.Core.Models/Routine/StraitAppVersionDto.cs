using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
    public class StraitAppVersionDto
    {
        public string Version { get; set; }
        public string Version_code { get; set; }
        public string Renewal { get; set; }
        public string Download_url { get; set; }
        public string Package_size { get; set; }
        public int Active_remind { get; set; }
    }
}
