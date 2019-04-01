using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models
{
    public class StraitAliPaySetting
    {
        /// <summary>
        /// 应用Id
        /// </summary>
        public string App_id { get; set; }

        /// <summary>
        /// 签名方式
        /// </summary>
        public string Sign_type { get; set; }

        /// <summary>
        /// 应用私钥
        /// </summary>
        public string PrivateKeyPem { get; set; }

        /// <summary>
        /// 支付宝服务器主动通知商户收银台里指定的页面http/https路径
        /// </summary>
        public string Notify_url { get; set; }

        /// <summary>
        /// 调用的接口版本，固定为：1.0
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 销售产品码，商家和支付宝签约的产品码
        /// </summary>
        public string Product_code { get; set; }

        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// 支付宝应用网关
        /// </summary>
        public string Gatewayurl { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public string CharSet { get; set; }

        public string Return_url { get; set; }
    }
}
