using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZsqApp.Core.Infrastructure.SysEnum
{
    //**************操作记录******************
    //创建时间：2018.01.03
    //作者：陶林辉
    //内容描述：Redis 枚举
    //***************************************
    public enum RedisEnum
    {
        /// <summary>
        /// 第一个库
        /// </summary>
        One,

        /// <summary>
        /// 第二个库
        /// </summary>
        Two,

        /// <summary>
        /// 第三个库
        /// </summary>
        Three,

        /// <summary>
        /// 第四个库
        /// </summary>
        Four,

        /// <summary>
        /// 第五个库
        /// </summary>
        /// 
        Five,

        /// <summary>
        /// 第六个库
        /// </summary>
        Six,

        /// <summary>
        /// 第七个库
        /// </summary>
        Seven,

        /// <summary>
        /// 第八个库
        /// </summary>
        Eight,

        /// <summary>
        /// 第九个库
        /// </summary>
        Nine,

        /// <summary>
        /// 第十个库
        /// </summary>
        Ten,

        /// <summary>
        /// 第十一个库
        /// </summary>
        Eleven,

        /// <summary>
        /// 第十二个库
        /// </summary>
        Twelve,

        /// <summary>
        /// 第十三个库
        /// </summary>
        Thirteen,

        /// <summary>
        /// 第十四个库
        /// </summary>
        Fourteen,

        /// <summary>
        /// 第十五个库
        /// </summary>
        Fifteen,

        /// <summary>
        /// 第十六个库
        /// </summary>
        Sixteen,

        /// <summary>
        /// 配置文件指定的库
        /// </summary>
        Default
    }

    public enum RedisFolderEnum
    {
        /// <summary>
        /// 根目录
        /// </summary>
        [Description("")]
        Root,

        /// <summary>
        /// APP
        /// </summary>
        [Description("APPKEY")]
        App_config,

        /// <summary>
        /// 短
        /// </summary>
        [Description("sessionToken")]
        sessionToken,

        /// <summary>
        /// 验证码
        /// </summary>
        [Description("code")]
        code,

        /// <summary>
        /// 长令牌
        /// </summary>
        [Description("Token")]
        token,

        /// <summary>
        /// 数据统计
        /// </summary>
        [Description("渠道树")]
        ChannelTree,
    }
}
