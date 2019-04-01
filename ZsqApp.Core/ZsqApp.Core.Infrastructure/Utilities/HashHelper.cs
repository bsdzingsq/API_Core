using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZsqApp.Core.Infrastructure.Utilities
{
    public class HashHelper
    {
        public static string Md5(string myString)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t2 = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(myString))).ToUpper();
            return t2.Replace("-", "");
        }

        public static string Sha1(string str)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var bytes = Encoding.UTF8.GetBytes(str);
            var hash = sha1.ComputeHash(bytes);

            var hashStr = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return hashStr;
        }
    }
}
