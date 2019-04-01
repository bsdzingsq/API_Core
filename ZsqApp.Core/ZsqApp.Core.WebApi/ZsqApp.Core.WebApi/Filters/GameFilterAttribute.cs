using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Filters
{
    /// <summary>
    /// 游戏通知过滤器
    /// </summary>
    public class GameFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Ons the action executing.
        /// </summary>
        /// <param name="context">Filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            GameRequestViweModel request = null;
            var _sys = context.HttpContext.RequestServices.GetService<ISystems>();
            string sign = string.Empty; //本地签名信息
            var route = context.RouteData.Values;
            var controller = route["controller"];
            var action = route["action"];
            try
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(context.HttpContext.Request.Body))
                {
                    request = JsonHelper.DeserializeJsonToObject<GameRequestViweModel>(sr.ReadToEnd());

                }
            }
            catch (Exception ex)
            {
                //缺少公共参数
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.LackParameter, null, _sys, "", "", "");
                context.Result = new JsonResult(response);
                return;
            }
            if (string.IsNullOrEmpty(request.appKey) || string.IsNullOrEmpty(request.sign)
                || string.IsNullOrEmpty(request.requestId) || string.IsNullOrEmpty(request.timestamp)
                || string.IsNullOrEmpty(request.nonce) || string.IsNullOrEmpty(request.signType)
                )
            {
                //公共参数必填项为空
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.EmptyParameter, null, _sys, "", "", "");
                context.Result = new JsonResult(response);
                return;

            }
            long time = 0;
            try
            {
                time = long.Parse(request.timestamp);
                time = TimeHelper.ConvertDateTimeToInt(DateTime.Now) - time;
                if (time > 1200)
                {
                    //非法时间戳
                    GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.TimestampErr, null, _sys, "", "", "");
                    context.Result = new JsonResult(response);
                    return;
                }
            }
            catch (Exception)
            {
                //非法时间戳
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.TimestampErr, null, _sys, request.appKey, request.requestId, request.nonce);
                context.Result = new JsonResult(response);
                return;
            }
            var appConfigDto = _sys.GetAppConfig(request.appKey);
            if (appConfigDto == null)
            {
                //无效appkey
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.AppKey, null, _sys, request.appKey, request.requestId, request.nonce);
                context.Result = new JsonResult(response);
                return;
            }
            sign = _sys.Sha512Encode($"{appConfigDto.Keys}{appConfigDto.Secret}{request.data}{request.nonce}{request.timestamp}");
            if (!request.sign.Equals(sign))
            {
                //签名验证失败
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.SignCheckErr, null, _sys, request.appKey, request.requestId, request.nonce);
                context.Result = new JsonResult(response);
                return;
            }
            if (!string.IsNullOrEmpty((string)request.data) && request.data != null)
            {
                (string json, var code) = _sys.Base64Decode(Encoding.UTF8, (string)request.data);
                if (code != SysCode.Ok)
                {
                    //base64解码失败
                    GameResponesViewModel<Object> response = new GameResponesViewModel<object>(SysCode.Base64DecodeErr, null, _sys, request.appKey, request.requestId, request.nonce);
                    context.Result = new JsonResult(response);
                    return;
                }
                else
                {
                    request.data = JsonHelper.DeserializeJsonToObject<dynamic>(json);
                }
            }
            var claimsIdentity = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name,JsonHelper.SerializeObject(request)),
            }, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.HttpContext.User = claimsPrincipal;
            if (true)
            {
                GameResponesViewModel<Object> response = new GameResponesViewModel<object>
               (_sys.CheckParameters(request.data, action.ToString()), null, _sys, request.appKey, request.requestId, request.nonce);
                if (response.code != 0)
                {
                    context.Result = new JsonResult(response);
                    return;
                }
            }
        }

    }
}
