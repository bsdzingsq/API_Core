using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class IbcGameInfoEntity
    {
        [Key]
        public long id { get; set; }
        public string channelid { get; set; }
        public string Register_gameid { get; set; }
        public string Ibc_gameid { get; set; }
        public string Default_gameid { get; set; }
        public int Status { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
    }
}
