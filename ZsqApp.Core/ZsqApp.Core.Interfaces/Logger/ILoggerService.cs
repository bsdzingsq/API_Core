using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity.MongoEntity;

namespace ZsqApp.Core.Interfaces.Logger
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：mongo日志接口定义
    //***************************************
    public interface ILoggerService
    {
        /// <summary>
        /// 异常日志
        /// </summary>
        /// <param name="logs"></param>
        void WriteException(ExceptionLogs logs);

        /// <summary>
        /// 访问日志
        /// </summary>
        /// <param name="log"></param>
        void WriteApiLog(AuditLogs log);
    }
}
