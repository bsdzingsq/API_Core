using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models.Headers;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Filters
{
    /// An abstract filter that asynchronously surrounds execution of the action and the action result. Subclasses
    /// should override
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext)" />
    /// ,
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext)" />
    /// or
    /// 
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)" />
    /// but not
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate)" />
    /// and either of the other two.
    /// Similarly subclasses should override
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecuting(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext)" />
    /// ,
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecuted(Microsoft.AspNetCore.Mvc.Filters.ResultExecutedContext)" />
    /// or
    /// 
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ResultExecutionDelegate)" />
    /// but not
    /// <see cref="M:Microsoft.AspNetCore.Mvc.Filters.ActionFilterAttribute.OnResultExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ResultExecutingContext,Microsoft.AspNetCore.Mvc.Filters.ResultExecutionDelegate)" />
    /// and either of the other two.
    public class ActionApiFilterAttribute : ActionFilterAttribute
    {



        /// <summary>
        /// 出
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// OnActionExecuting 重写，用于验证参数
        /// author：陶林辉
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Attribute 只能这样获取注入接口对象
            var _sys = context.HttpContext.RequestServices.GetService<ISystems>();
            var _routine = context.HttpContext.RequestServices.GetService<IRoutine>();
            RequestViewModel request = null;
            string sign = string.Empty; //本地签名信息
            var route = context.RouteData.Values;
            var controller = route["controller"];
            var action = route["action"];
            #region  请求头参数认证
            try
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);//FromBody 不加读取不到参数
                using (var sr = new StreamReader(context.HttpContext.Request.Body))
                {
                    request = JsonHelper.DeserializeJsonToObject<RequestViewModel>(sr.ReadToEnd());  //业务参数，加密方式
                }
                request.HendInfo = JsonHelper.DeserializeJsonToObject<HeadConten>(context.HttpContext.Request.Headers["HeadConten"]); //请求头
                request.Client = JsonHelper.DeserializeJsonToObject<ClientInfo>(context.HttpContext.Request.Headers["Client"]);//客户端信息
            }
            catch (Exception)
            {
                //缺少参数
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.LackParameter, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            }
            /*请求头校验*/
            if (string.IsNullOrEmpty(request.HendInfo.Sign) || string.IsNullOrEmpty(request.HendInfo.Timestamp)
                || string.IsNullOrEmpty(request.HendInfo.UuId) || string.IsNullOrEmpty(request.HendInfo.AppKey)
                || string.IsNullOrEmpty(request.Client.Channel) || string.IsNullOrEmpty(request.Client.DeviceCode)
                || string.IsNullOrEmpty(request.Client.Gps) || string.IsNullOrEmpty(request.Client.OsType)
                || string.IsNullOrEmpty(request.Client.OsVersion) || string.IsNullOrEmpty(request.Client.Platform)
                || string.IsNullOrEmpty(request.Client.UserAgent) || string.IsNullOrEmpty(request.Client.Version)
                || string.IsNullOrEmpty(request.Client.VersionCode)
                )
            {
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.EmptyParameter, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            }
            if (!_routine.JudgeVersionStatus(request.Client.Version))
            {
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.VersionIsNo, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            }
            /*appkey校验*/
            var appConfigDto = _sys.GetAppConfig(request.HendInfo.AppKey);
            if (appConfigDto == null)
            {
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.AppKey, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            };
            request.Secret = appConfigDto.Secret;
            /*时间戳校验*/
            long ltime = TimeHelper.ConvertDateTimeToInt(DateTime.Now) - long.Parse(request.HendInfo.Timestamp);
            if (ltime > 120000)
            {

                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.TimestampErr, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
            }
            /*主参数校验*/
            if (request == null || request.Encrypt == null || request.Data == null)
            {
                //缺少参数
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.LackParameter, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            }
            /*签名验证 appKey+appSecret+data+uuId+timestamp*/
            sign = _sys.Md5Encode($"{appConfigDto.Keys}{appConfigDto.Secret}{request.Data}{request.HendInfo.UuId}{request.HendInfo.Timestamp}");
            if (!request.HendInfo.Sign.Equals(sign))
            {
                ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.SignCheckErr, null, request.Encrypt, _sys, "");
                context.Result = new JsonResult(response);
                return;
            }
            /*业务参数解密*/
            if (request.Encrypt == "0" || request.Encrypt == "")//base编码
            {
                string strTemp = request.Data.ToString();
                (string strJson, var vCode) = _sys.Base64Decode(Encoding.UTF8, strTemp);
                if (vCode != SysCode.Ok)
                {
                    //64解码失败
                    ResponseViewModel<object> response = new ResponseViewModel<object>(SysCode.Base64DecodeErr, null, request.Encrypt, _sys, "");
                    context.Result = new JsonResult(response);
                    return;
                }
                //这里需要增加strJson是否是json格式的判断
                request.Data = JsonHelper.DeserializeJsonToObject<dynamic>(strJson);
            }
            else

            {
                request.Data = JsonHelper.DeserializeJsonToObject<dynamic>(_sys.AesDecrypt(request.Data.ToString(), appConfigDto.Secret));
            }
            var claimsIdentity = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name,JsonHelper.SerializeObject(request)),
            }, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.HttpContext.User = claimsPrincipal;
            #endregion
            /*业务参数校验*/
            if (true)
            {
                ResponseViewModel<object> response = new ResponseViewModel<object>(_sys.CheckParameters(request.Data, action.ToString()), null, request.Encrypt, _sys, "");
                if (response.Code != 0)
                {
                    context.Result = new JsonResult(response);
                    return;
                }
            }
        }
    }
}