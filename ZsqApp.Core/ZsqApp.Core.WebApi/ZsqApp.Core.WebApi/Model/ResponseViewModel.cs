using System;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using System.Text;
namespace ZsqApp.Core.WebApi.Model
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：标准返回
    //***************************************
    public class ResponseViewModel<T>
    {
        protected readonly ISystems _sys; /**/
        /// <summary>
        /// 返回值构造函数
        /// author:陶林辉
        /// </summary>
        /// <param name="sysCode">错误枚举</param>
        /// <param name="data">返回业务参数</param>
        /// <param name="encrypt">加密方式</param>
        public ResponseViewModel(SysCode sysCode, T data, string encrypt, ISystems sys, string secret)
        {
            _sys = sys;
            Code = (int)sysCode;
            Message = EnumExtention.GetDescription(sysCode);
            Datetime = DateTime.Now.ToLocalTime().ToString();
            Timestamp = TimeHelper.ConvertDateTimeToInt(DateTime.Now.ToLocalTime());
            if (encrypt == "0")
            {
                if (data != null)
                {
                    this.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(data));
                }

            }
            else if (encrypt == "1")
            {
                if (data != null)
                {
                    this.Data = _sys.AesEncrypt(secret, JsonHelper.SerializeObject(data));
                }
            }
            this.Encrypt = encrypt;
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 接口所在服务器当前时间
        /// </summary>
        public string Datetime { get; set; }

        /// <summary>
        /// 距离UTC 1970-01-01 00:00:00的秒数
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 具体返回参数
        /// </summary>
        public dynamic Data { get; set; }

        /// <summary>
        /// 是否加密，和请求对应
        /// </summary>
        public string Encrypt { get; set; }
    }


}
