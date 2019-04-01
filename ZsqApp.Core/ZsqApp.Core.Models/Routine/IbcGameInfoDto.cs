using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
    public class IbcGameInfoDto
    {
        public long id { get; set; }
        public string channelid { get; set; }
        public string user_gameid { get; set; }
        public string ibc_gameid { get; set; }
        public string default_gameid { get; set; }
        public int is_show { get; set; }
    }
}
