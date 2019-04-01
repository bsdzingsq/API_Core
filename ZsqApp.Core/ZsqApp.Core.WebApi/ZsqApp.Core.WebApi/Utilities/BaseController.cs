using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZsqApp.Core.Entity.MongoEntity;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Logger;
using ZsqApp.Core.Models.Headers;
using ZsqApp.Core.WebApi.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZsqApp.Core.WebApi.Utilities
{

    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.Any(m => typeof(RequestViewModel).IsInstanceOfType(m.Value)) ||
                !context.HttpContext.Request.Headers.Keys.Contains("HeadConten") ||
                !context.HttpContext.Request.Headers.Keys.Contains("Client")
                )
            {
                return;
            }
            var vItem = new AuditLogs
            {
                Action = context.ActionDescriptor.DisplayName,
                Path = context.HttpContext.Request.Path,
                Requset = JsonHelper.SerializeObject(context.ActionArguments),
                Time = DateTime.Now.ToLocalTime(),
                Stopwatch = Stopwatch.StartNew(),
                HeadConten = context.HttpContext.Request.Headers["HeadConten"],
                ClientInfo = context.HttpContext.Request.Headers["Client"],
                Ip = context.HttpContext.Request.Headers["X-Original-For"].FirstOrDefault(),
            };
            context.HttpContext.Items["auditlog"] = vItem;

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var vLog = Request.HttpContext.RequestServices.GetService(typeof(ILoggerService)) as ILoggerService;
            if (vLog == null) return;
            if (!(context.HttpContext.Items["auditlog"] is AuditLogs item)) return;
            var vDuration = item.Stopwatch.ElapsedMilliseconds;
            var vResult = context.Result as ObjectResult;
            item.Duration = vDuration;
            item.Response = JsonHelper.SerializeObject(vResult?.Value);
            item.Exception = context.Exception;
            vLog.WriteApiLog(item);
        }
    }
}
