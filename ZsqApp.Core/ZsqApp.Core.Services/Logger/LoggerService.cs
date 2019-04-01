using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity.MongoEntity;
using ZsqApp.Core.Interfaces.Logger;
using MongoDB.Driver;
using System.Linq;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Models;

namespace ZsqApp.Core.Services.Logger
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：mongo日志接口实现
    //***************************************
    public class LoggerService : ILoggerService
    {
        private readonly IOptions<MongoSettings> _options;

        private readonly MongoSettings _appSettings;

        public LoggerService(IOptions<MongoSettings> options)
        {
            this._options = options;
            _appSettings = options.Value;
        }

        /// <summary>
        /// 异常写入
        /// </summary>
        /// <param name="logs"></param>
        public void WriteException(ExceptionLogs logs)
        {
            var client = new MongoClient(_options.Value.MongoConnStr);
            var db = client.GetDatabase(_options.Value.MongoDbName);
            var collection = db.GetCollection<ExceptionLogs>("ExceptionLogs");
            collection.InsertOne(logs);
        }


        /// <summary>
        /// 访问日志
        /// </summary>
        /// <param name="log"></param>
        public void WriteApiLog(AuditLogs log)
        {
            var client = new MongoClient(_appSettings.MongoConnStr);
            var db = client.GetDatabase(_appSettings.MongoDbName);
            var collection = db.GetCollection<AuditLogs>("AuditLogs");
            collection.InsertOne(log);
        }
    }
}
