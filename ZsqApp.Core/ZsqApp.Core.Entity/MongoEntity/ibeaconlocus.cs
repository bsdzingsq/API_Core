using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZsqApp.Core.Models.Headers;

namespace ZsqApp.Core.Entity.MongoEntity
{
    public class ibeaconlocus
    {
        [BsonId]
        public ObjectId _id { get; set; }
        /// <summary>
        /// 当前渠道
        /// </summary>
        public string downid { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public long userId { get; set; }

        /// <summary>
        /// icb信息
        /// </summary>
        public List<ibcinfoList> iBeacons { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// gps
        /// </summary>
        public string gps { get; set; }

        public string timestamp { get; set; }

        public string token { get; set; }
        public string userOpenid { get; set; }
        public string phoneType { get; set; }
        public string gameType { get; set; }
        public string ibcChannel { get; set; }
        public string channelId { get; set; }

        public DateTime createTime { get; set; }

        public class ibcinfoList
        {          
            public string name { get; set; }
            public string uuid { get; set; }
            public int minor { get; set; }
            public int major { get; set; }
            public int txPower { get; set; }
            public int rssi { get; set; }
            public string adress { get; set; }
            public string timestamp { get; set; }
            public int battery { get; set; }
        }
    }
}
