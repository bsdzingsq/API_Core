using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System;

namespace ZsqApp.Core.Infrastructure.Extentions
{
    /// <summary>
    /// 字符串扩展类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 用于判断是否为空字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBlank(this string s)
        {
            return s == null || (s.Trim().Length == 0);
        }

        /// <summary>
        /// 用于判断是否为空字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNotBlank(this string s)
        {
            return !s.IsBlank();
        }

        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetExt(this string s)
        {
            var ret = string.Empty;
            if (!s.Contains('.')) return ret;
            var temp = s.Split('.');
            ret = temp[temp.Length - 1];

            return ret;
        }

        /// <summary>
        /// 验证QQ格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsQq(this string s)
        {
            return s.IsBlank() || Regex.IsMatch(s, @"^[1-9]\d{4,15}$");
        }

        /// <summary>
        /// 验证身份证格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsIdCard(this string s)
        {
            return s.IsBlank() || Regex.IsMatch(s, @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$");
        }

        /// <summary>
        /// 验证渠道号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsChannelId(this string s)
         {
             return s.IsBlank() || Regex.IsMatch(s, @"^\d{15}$");
        }
        
        /// <summary>
        /// 匹配8到12位字符
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPwd(this string s)
        {
            return s.IsBlank() || Regex.IsMatch(s, @"^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,12}$");
            
        }

        /// <summary>
        /// 判断是否为有效的Email地址
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmail(this string s)
        {
            if (!s.IsBlank())
            {
                //return Regex.IsMatch(s,
                //         @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
                //         @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
                const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
                return Regex.IsMatch(s, pattern);
            }
            return false;
        }

        /// <summary>
        /// 验证是否是合法的电话号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsPhone(this string s)
        {
            if (!s.IsBlank())
            {

                return Regex.IsMatch(s, @"^\+?((\d{2,4}(-)?)|(\(\d{2,4}\)))*(\d{0,16})*$");
            }
            return false;
        }

        /// <summary>
        /// 验证是否是合法的手机号码
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            if (!s.IsBlank())
            {
                return Regex.IsMatch(s, @"^\+?\d{0,4}?[1][3-9]\d{9}$");
            }
            return false;
        }

        /// <summary>
        /// 验证是否是合法的邮编
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsZipCode(this string s)
        {
            if (!s.IsBlank())
            {
                return Regex.IsMatch(s, @"[1-9]\d{5}(?!\d)");
            }
            return true;
        }

        /// <summary>
        /// 验证是否是合法的传真
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsFax(this string s)
        {
            if (!s.IsBlank())
            {
                return Regex.IsMatch(s, @"(^[0-9]{3,4}\-[0-9]{7,8}$)|(^[0-9]{7,8}$)|(^\([0-9]{3,4}\)[0-9]{3,8}$)|(^0{0,1}13[0-9]{9}$)");
            }
            return true;
        }

        /// <summary>
        /// 检查字符串是否为有效的int数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool Isint(this string val)
        {
            if (IsBlank(val))
                return false;
            int k;
            return int.TryParse(val, out k);
        }

        /// <summary>
        /// 字符串转数字，未转换成功返回0
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToInt(this string val)
        {
            if (IsBlank(val))
                return 0;
            int k;
            return int.TryParse(val, out k) ? k : 0;
        }

        /// <summary>
        /// 检查字符串是否为有效的INT64数字
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsInt64(this string val)
        {
            if (IsBlank(val))
                return false;
            long k;
            return long.TryParse(val, out k);
        }

        /// <summary>
        /// 检查字符串是否为有效的Decimal
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsDecimal(this string val)
        {
            if (IsBlank(val))
                return false;
            decimal d;
            return decimal.TryParse(val, out d);
        }

        /// <summary>
        /// 判断渠道号级别
        /// </summary>
        /// <returns></returns>
        public static int IsChannelIdLevel(string channelId)
        {
            int ilevel = 0;
            if (Convert.ToInt32(channelId.Substring(5, 10)) == 0)
            {
                ilevel = 1;
            }
            else if (Convert.ToInt32(channelId.Substring(10, 5)) == 0)
            {
                ilevel = 2;
            }
            else
            {
                ilevel = 3;
            }
            return ilevel;
        }

        /// <summary>
        /// 根据级别修改渠道号
        /// </summary>
        /// <returns></returns>
        public static string UpdateChannelId(string channelId,int level)
        {
            string strNewchannelId = "";
            if (level==1)
            {
                strNewchannelId = channelId.Substring(0,5)+"0000000000";
            }
            else if(level==2)
            {
                strNewchannelId = channelId.Substring(0, 10) + "00000";
            }
            else
            {
                strNewchannelId = channelId;
            }
            return strNewchannelId;
        }

        /// <summary>
        /// 字符串截取
        /// </summary>
        /// <param name="val"></param>
        /// <param name="startIndex"></param>
        /// <param name="overIndex"></param>
        /// <returns></returns>
        public static string CutOutString(string val, int startIndex,  int overIndex)
        {
            if (IsBlank(val))
                return null;
            return val.Substring(startIndex, overIndex);
        }
    }
}
