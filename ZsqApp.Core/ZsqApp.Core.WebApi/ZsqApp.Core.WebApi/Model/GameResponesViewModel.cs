using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.WebApi.Model
{
    /// <summary>
    /// 游戏通知标准返回
    /// </summary>
    public class GameResponesViewModel<T>
    {
        protected readonly ISystems _sys;
        public GameResponesViewModel(SysCode sysCode, T data, ISystems sys, string appkey, string requestId, string nonce)
        {

            _sys = sys;
            string appSecret = _sys.GetAppConfig(appkey).Secret;
            code = (int)sysCode;
            message = EnumExtention.GetDescription(sysCode);
            timestamp = TimeHelper.ConvertDateTimeToInt(DateTime.Now.ToLocalTime()).ToString();
            if (data != null)
            {
                this.data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(data));
                sign = _sys.Sha512Encode($"{appkey}{appSecret}{this.data}{nonce}{timestamp}");
            }
            else
            {
                sign = _sys.Sha512Encode($"{appkey}{appSecret}{nonce}{timestamp}");
            }
            this.requestId = requestId;
            this.appKey = appkey;
            this.nonce = nonce;
            page = null; //用不上
            signType = "sha_512";
        }
        public int code { get; set; }

        public string message { get; set; }

        public string appKey { get; set; }

        public string requestId { get; set; }

        public string timestamp { get; set; }

        public string nonce { get; set; }

        public string sign { get; set; }

        public string signType { get; set; }

        public string data { get; set; }

        public PageResult page { get; set; }
    }
}
