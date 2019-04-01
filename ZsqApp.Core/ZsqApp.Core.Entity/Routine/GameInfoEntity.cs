using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class GameInfoEntity
    {
        [Key]
        public long Id { get; set; }
        public int Game_type_id { get; set; }
        public string Game_Name { get; set; }
        public string Target_url { get; set; }
        public string Icon_url { get; set; }
        public string Description { get; set; }
        public int Url_type { get; set; }
        public int Sort { get; set; }
        public int Stateus { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

    }
}
