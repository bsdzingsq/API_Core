using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ZsqApp.Core.Entity.MongoEntity
{
    /// <summary>
    /// 异常日志
    /// </summary>
    public class ExceptionLogs
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public string Path { get; set; }

        public string Action { get; set; }

        public string Message { get; set; }

        public string Stack { get; set; }
    }
}
