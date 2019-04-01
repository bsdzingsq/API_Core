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
    /// 
    /// </summary>
    public class ExternalActionApiFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Ons the action executing.
        /// </summary>
        /// <param name="context">Filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ExternalRequesViewModel RequestObj = null;
            var errCode = SysCode.Ok;
            var _sys = context.HttpContext.RequestServices.GetService<ISystems>();
            ExternalResponesViewModel<object> Response = null;
            var route = context.RouteData.Values;
            var action = route["action"];
            string md5 = string.Empty;
            try
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);//FromBody 不加读取不到参数
                using (var sr = new StreamReader(context.HttpContext.Request.Body))
                {
                    RequestObj = JsonHelper.DeserializeJsonToObject<ExternalRequesViewModel>(sr.ReadToEnd());  //业务参数，加密方式

                }
            }
            catch (Exception)
            {
                errCode = SysCode.LackParameter;
                Response = new ExternalResponesViewModel<object>(errCode, _sys, null);
                context.Result = new JsonResult(Response);
                //缺少参数
                return;
            }

            /*appkey校验*/
            var appConfigDto = _sys.GetAppConfig(RequestObj.AppKey);
            if (appConfigDto == null)
            {
                errCode = SysCode.AppKey;
                Response = new ExternalResponesViewModel<object>(errCode, _sys, null);
                context.Result = new JsonResult(Response);
                return;
            };

            /*时间戳校验 */
            long ltime = TimeHelper.ConvertDateTimeToInt(DateTime.Now) - long.Parse(RequestObj.Stimestamp);
            if (ltime > 120000)
            {

                errCode = SysCode.TimestampErr;
                Response = new ExternalResponesViewModel<object>(errCode, _sys, null);
                context.Result = new JsonResult(Response);
                return;

            }
            /*签名验证 appKey+appSecret+data+uuId+timestamp*/
            md5 = _sys.Md5Encode($"{appConfigDto.Keys}{appConfigDto.Secret}{RequestObj.Data}{RequestObj.Stimestamp}");
            if (!RequestObj.Sign.Equals(md5))
            {
                errCode = SysCode.SignCheckErr;
                Response = new ExternalResponesViewModel<object>(errCode, _sys, null);
                context.Result = new JsonResult(Response);
                return;
            }
            string strTemp = RequestObj.Data.ToString();
            (string strJson, var code) = _sys.Base64Decode(Encoding.UTF8, strTemp);
            if (code != SysCode.Ok)
            {
                //64解码失败
                errCode = SysCode.Base64DecodeErr;
                Response = new ExternalResponesViewModel<object>(errCode, _sys, null);
                context.Result = new JsonResult(Response);
                return;
            }
            RequestObj.Data = JsonHelper.DeserializeJsonToObject<dynamic>(strJson);
            //这里需要增加strJson是否是json格式的判断

            var claimsIdentity = new ClaimsIdentity(new Claim[] {
            new Claim(ClaimTypes.Name,JsonHelper.SerializeObject(RequestObj)),
            }, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.HttpContext.User = claimsPrincipal;
            /*业务参数校验*/
            if (true)
            {
                ExternalResponesViewModel<object> response = new ExternalResponesViewModel<object>(_sys.CheckParameters(RequestObj.Data, action.ToString()), _sys, null);
                if (response.Code != 0)
                {
                    context.Result = new JsonResult(response);
                    return;
                }
            }
        }
    }
}
