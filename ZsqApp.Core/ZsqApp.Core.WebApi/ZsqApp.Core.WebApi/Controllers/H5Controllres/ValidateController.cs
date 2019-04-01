using System;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.Validate;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{
    /// <summary>
    /// H5提供用户相关接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/validate")]
    [EnableCors("any")]
    public class ValidateController : Controller
    {
        #region
        /// <summary>
        /// 系统
        /// </summary>
        protected readonly ISystems _sys;

        private readonly ValidateSetting _validate;

        /// <summary>
        ///  构造函数注入
        /// </summary>
        /// <param name="sys"></param>
        /// <param name="validate"></param>
        public ValidateController(ISystems sys, IOptions<ValidateSetting> validate)
        {
            _sys = sys;
            _validate = validate.Value;
        }
        #endregion
        /// <summary>
        /// 生成图形码
        /// author:白尚德
        /// update:修改了小部分逻辑，修改了参数命名   :陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("getValidate")]
        public H5ResponseViewModel<ValidateCodeView> GetValidate([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<ValidateCodeView> response = null;
            ValidateCode validatecode = new ValidateCode();
            ValidateSetting _imgurl = new ValidateSetting();
            ValidateCodeView validate = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "GetValidate");
            if (sysCode == SysCode.Ok)
            {
                dynamic dyParameter = Parameters.data;
                string strCodeId = dyParameter.codeId;
                if (!string.IsNullOrEmpty(strCodeId))
                {
                    RedisHelper.KeyDelete(strCodeId, RedisFolderEnum.code, RedisEnum.Four); //删除原来的图片 
                }
                validatecode.key = Guid.NewGuid().ToString();
                validatecode.CreateValidateImage(4);
                validate = new ValidateCodeView
                {
                    name = validatecode.key,
                    imgUrl = $"{_validate.Url}{validatecode.key}.gif"
                };

            }
            else
            {
                sysCode = SysCode.Err;
            }
            response = new H5ResponseViewModel<ValidateCodeView>(sysCode, validate);
            return response;
        }

        /// <summary>
        /// 校检验证码
        /// author:白尚德
        /// update:修改了小部分逻辑，修改了参数命名  :陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("contrastCode")]
        public H5ResponseViewModel<ValidateCodeView> ContrastCode([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<ValidateCodeView> response = null;
            ValidateCode validatecode = new ValidateCode();
            var sysCode = _sys.CheckParameters(Parameters.data, "ContrastCode");
            if (sysCode == SysCode.Ok)
            {
                string key = Parameters.data.name;
                string results = Parameters.data.result;
                string strSysCode = RedisHelper.StringGet(key, RedisFolderEnum.code, RedisEnum.Four);//缓存地址
                string[] strList = null;
                int iResule = 0;
                if (!string.IsNullOrEmpty(strSysCode) && strSysCode != null)
                {
                    #region
                    //取出验证码
                    //if (strSysCode.Contains("x"))
                    //{
                    //    strList = strSysCode.Split('x');
                    //    iResule = (int.Parse(strList[0])) * (int.Parse(strList[1]));
                    //    if (results != iResule.ToString())
                    //    {
                    //        //输入错误
                    //        sysCode = SysCode.CodeErr;
                    //    }
                    //}
                    //else if (strSysCode.Contains("+"))
                    //{
                    //    strList = strSysCode.Split('+');
                    //    iResule = (int.Parse(strList[0])) + (int.Parse(strList[1]));
                    //    if (results != iResule.ToString())
                    //    {
                    //        //输入错误
                    //        sysCode = SysCode.CodeErr;
                    //    }
                    //}
                    #endregion
                    // if (results.ToUpper() == strSysCode.ToString().ToUpper())
                    if (results.ToString() == strSysCode.ToString())
                    {
                        sysCode = SysCode.Ok;
                    }
                    else
                    {
                        //输入错误
                        sysCode = SysCode.CodeErr;
                    }
                }
                else
                {
                    //没取到缓存
                    sysCode = SysCode.Err;
                }
            }
            else
            {
                sysCode = SysCode.Err;
            }
            response = new H5ResponseViewModel<ValidateCodeView>(sysCode, null);
            return response;
        }
    }
}