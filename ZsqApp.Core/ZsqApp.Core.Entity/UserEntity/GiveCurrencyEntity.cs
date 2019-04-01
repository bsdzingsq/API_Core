using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Entity.UserEntity
{

    public class GiveCurrencyEntity
    {
        public long id { get; set; }

        public long userId { get; set; }

        public string key { get; set; }

        public DateTime giveTime { get; set; }

        public string channelId { get; set; }


    }
}
