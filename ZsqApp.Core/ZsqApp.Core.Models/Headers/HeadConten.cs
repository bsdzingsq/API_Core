using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Headers
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：请求头认证信息
    //***************************************
    public class HeadConten
    {
        /// <summary>
        /// 分配的appkey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Sign { get; set; }

        /// <summary>
        /// 距离UTC 1970-01-01 00:00:00的秒数
        /// </summary>
        public string Timestamp { get; set; }


        /// <summary>
        /// 客户端生成的随机字符串 用于去重处理
        /// </summary>
        public string UuId { get; set; }

        /// <summary>
        /// 登陆时需要
        /// </summary>
        public string UserOpenId { get; set; }

        /// <summary>
        /// 登陆时需要
        /// </summary>
        public string SessionToken { get; set; }


    }
}
