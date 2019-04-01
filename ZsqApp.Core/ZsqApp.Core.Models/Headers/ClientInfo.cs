using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Headers
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：请求头客户端信息
    //***************************************
    public class ClientInfo
    {
        /// <summary>
        /// 终端设备型号(各种手机型号等)
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 操作系统类型
        /// </summary>
        public string OsType { get; set; }

        /// <summary>
        /// 操作系统版本
        /// </summary>
        public string OsVersion { get; set; }

        /// <summary>
        /// 类似浏览器的UA概念
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 客户端平台 iOS/Android/h5
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 客户端版本号
        /// </summary>
        public string VersionCode { get; set; }

        /// <summary>
        ///客户端来源渠道
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// 用户坐标
        /// </summary>
        public string Gps { get; set; }

    }
}
