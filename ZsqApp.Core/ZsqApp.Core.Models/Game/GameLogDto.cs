using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Game
{
    public class GameLogDto
    {

        public string Game_key { get; set; }

        public string Game_setId { get; set; }

        public string Order_id { get; set; }

        public long User_id { get; set; }

        public decimal Amount { get; set; }

        public string Operate_time { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 1投注，2派奖，3退款
        /// </summary>
        public int Types { get; set; }

        public DateTime Createtime { get; set; }
    }
}
