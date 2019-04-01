using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.ViewModel.Recharge
{
    public class AliPayView
    {
        /// <summary>
        /// 支付宝分配给开发者的应用ID
        /// </summary>
        public string App_id { get; set; }

        /// <summary>
        /// 接口名称
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求使用的编码格式，收银台默认为utf-8
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// 商户生成签名字符串所使用的签名算法类型，目前支持RSA
        /// </summary>
        public string Sign_type { get; set; }

        /// <summary>
        /// 商户请求参数的签名串
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 发送请求的时间，格式”yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 调用的接口版本，固定为：1.0
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 支付宝服务器主动通知商户收银台里指定的页面http/https路径，为固定值
        /// </summary>
        public string Notify_url { get; set; }

        /// <summary>
        /// 业务请求参数的集合，最大长度不限，除公共参数外所有请求参数都必须放在这个参数中传递
        /// </summary>
        public string Biz_content { get; set; }


        public string Format { get; set; }

    }
}
