using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Infrastructure.Sdk;
using ZsqApp.Core.Interfaces.Message;

namespace ZsqApp.Core.Services.Message
{
    //**************操作记录******************
    //创建时间：2018.1.25
    //作者：陶林辉
    //内容描述：荣联云平台
    //***************************************
    public class PhoneMessageService : IPhoneMessage
    {
        /// <summary>
        /// 短信验证码推送
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">推送手机号码</param>
        /// <param name="val">验证码</param>
        /// <param name="type">1短信，2语音</param>
        /// <param name="template">推送模板</param>
        public  bool MessageCode(string phone, string val, int type, string template = "196568")
        {
            Infrastructure.Sdk.CCPRestSDK api = new CCPRestSDK();
            /*服务器地址，服务器端口*/
            bool bIsInit = api.init("app.cloopen.com", "8883"); 
            string strRet = string.Empty;
            string strResult = string.Empty;
            /*主账号，主账号令牌,如果主页更换了token需要在这里更改令牌*/
            api.setAccount("8aaf070857acf7a70157adee1554019f", "587ffc830e3d46c59e70f907f50e78e0");
            api.setAppId("8a216da859aa5a950159abc6055a01f5"); /*应用ID*/
            try
            {
                Dictionary<string, object> RetData = null;
                if (type == 1) //短信
                {
                    /*手机号码，短信模板，验证码*/
                    RetData = api.SendTemplateSMS(phone, template, new string[] { val, "1" });
                }
                else
                {   /*手机号码，验证码，显示主叫号码，重复次数，回调地址*/
                    RetData = api.VoiceVerify(phone, val, null, "3", null);
                }
                //推送短信/电话
                strRet = api.getDictionaryData(RetData);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                string[] str_Temp = strRet.Split(';');
                strResult = str_Temp[0];
                strResult = strResult.Substring(strResult.IndexOf("=") + 1);
            }
            if (strResult != "000000")
            {
                //推送失败
                return false;
            }
            return true;
        }

        /// <summary>
        /// 海峡竞技短信验证码推送
        /// author：白尚德
        /// </summary>
        /// <param name="phone">推送手机号码</param>
        /// <param name="val">验证码</param>
        /// <param name="type">1短信，2语音</param>
        /// <param name="templates">推送模板</param>
        public bool StraitMessageCode(string phone, string val, int type, string templates = "377916")
        {
            Infrastructure.Sdk.CCPRestSDK api = new CCPRestSDK();
            /*服务器地址，服务器端口*/
            bool bIsInit = api.init("app.cloopen.com", "8883");
            string strRet = string.Empty;
            string strResult = string.Empty;
            /*主账号，主账号令牌,如果主页更换了token需要在这里更改令牌*/
            api.setAccount("8aaf070857acf7a70157adee1554019f", "587ffc830e3d46c59e70f907f50e78e0");
            api.setAppId("8aaf070866f7197701670ab5393309e7"); /*应用ID*/
            string template = "377916";
            try
            {
                Dictionary<string, object> RetData = null;
                if (type == 1) //短信
                {
                    /*手机号码，短信模板，验证码*/
                    RetData = api.SendTemplateSMS(phone, template, new string[] { val, "1" });
                }
                else
                {   /*手机号码，验证码，显示主叫号码，重复次数，回调地址*/
                    RetData = api.VoiceVerify(phone, val, null, "3", null);
                }
                //推送短信/电话
                strRet = api.getDictionaryData(RetData);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                string[] str_Temp = strRet.Split(';');
                strResult = str_Temp[0];
                strResult = strResult.Substring(strResult.IndexOf("=") + 1);
            }
            if (strResult != "000000")
            {
                //推送失败
                return false;
            }
            return true;
        }
    }
}
