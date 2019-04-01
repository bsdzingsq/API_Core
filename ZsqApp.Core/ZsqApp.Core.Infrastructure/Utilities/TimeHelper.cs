using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Infrastructure.Utilities
{
    /// <summary>
    /// 时间
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// 装换成时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetDateTime()
        {
            return DateTime.Now.ToLocalTime();
        }

        /// <summary>  
        /// 获取当前本地时间戳  
        /// </summary>  
        /// <returns></returns>        
        public static long GetCurrentTimeUnix()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
        }

        /// <summary>
        /// 计算时间天数
        /// </summary>
        /// <returns></returns>
        public static int Dateday(DateTime now, DateTime signtime)
        {
            DateTime dt1 = Convert.ToDateTime(now);
            DateTime dt2 = Convert.ToDateTime(signtime);
            TimeSpan ts = dt1 - dt2;
            return ts.Days;     //sub就是两天相差的天数
        }
    }
}
