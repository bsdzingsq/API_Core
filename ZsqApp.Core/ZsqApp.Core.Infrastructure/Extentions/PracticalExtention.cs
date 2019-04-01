using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Infrastructure.Extentions
{
    /// <summary>
    /// 实用工具
    /// </summary>
    public class PracticalExtention
    {
        /// <summary>
        /// 简易订单号
        /// </summary>
        /// <returns></returns>
        public static string getNumber()
        {

            string str = "ZSQ";
            string time = DateTime.Now.ToFileTimeUtc().ToString();// 128650040772968750
            return str + time;

        }
    }
}
