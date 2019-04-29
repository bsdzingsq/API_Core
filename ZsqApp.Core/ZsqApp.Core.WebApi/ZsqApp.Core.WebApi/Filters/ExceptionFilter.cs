using System;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZsqApp.Core.Entity.MongoEntity;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Logger;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Filters
{

    /// <summary>
    /// Exception filter.
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILog _log;

        public ExceptionFilter()
        {
            _log = LogManager.GetLogger(Startup.repository.Name, typeof(ExceptionFilter));
        }
        /// <summary>
        /// 异常捕获，
        /// author：陶林辉
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            var log = new ExceptionLogs
            {
                Message = context.Exception.Message,
                Stack = context.Exception.ToString(),
                Time = DateTime.Now,
                Path = context.HttpContext.Request.Path,
                Action = context.ActionDescriptor.DisplayName
            };
            var loggerRepository = (ILoggerService)context.HttpContext.RequestServices.GetService(typeof(ILoggerService));
            var sys =(ISystems)context.HttpContext.RequestServices.GetService(typeof(ISystems));
            loggerRepository.WriteException(log);
            _log.ErrorFormat("异常捕获:{0}", JsonHelper.SerializeObject(log));
            // context.Result = new JsonResult(new ResponseViewModel<object>(SysCode.Unknown,"{}",null, sys, ""));
            context.Result = new JsonResult(new UnknownViewModel<object>(SysCode.Unknown, sys, log.Message));
        }
    }
}
