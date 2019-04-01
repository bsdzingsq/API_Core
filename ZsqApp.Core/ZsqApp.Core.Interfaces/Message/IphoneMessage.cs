using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZsqApp.Core.Interfaces.Message
{
    //**************操作记录******************
    //创建时间：
    //作者：陶林辉
    //内容描述：荣联云平台
    //***************************************
    public interface IPhoneMessage
    {
        /// <summary>
        /// 短信验证码推送
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">推送手机号码</param>
        /// <param name="val">验证码</param>
        /// <param name="type">1短信，2语音</param>
        /// <param name="template">推送模板</param>
        bool MessageCode(string phone, string val, int type, string template = "196568");

        /// <summary>
        /// 海峡竞技短信验证码推送
        /// author：白尚德
        /// </summary>
        /// <param name="phone">推送手机号码</param>
        /// <param name="val">验证码</param>
        /// <param name="type">1短信，2语音</param>
        /// <param name="template">推送模板</param>
        bool StraitMessageCode(string phone, string val, int type, string template = "196568");

    }
}
