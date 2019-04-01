using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZsqApp.Core.Models.Headers;

namespace ZsqApp.Core.Entity.MongoEntity
{
    /// <summary>
    /// 接口日志
    /// </summary>
    public class AuditLogs
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)] //C#的驱动支持一个特性，将实体的时间属性上添加上这个特性并指时区就可以了。
        public DateTime Time { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// 请求地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Requset { get; set; }

        /// <summary>
        /// 客户端信息
        /// </summary>
        public string ClientInfo { get; set; }

        /// <summary>
        /// 请求头信息
        /// </summary>
        public string HeadConten { get; set; }

        public long Duration { get; set; }

        public Exception Exception { get; set; }

        [BsonIgnore]
        public Stopwatch Stopwatch { get; set; }

        public string Response { get; set; }
        public string Ip { get; set; }
    }
}
