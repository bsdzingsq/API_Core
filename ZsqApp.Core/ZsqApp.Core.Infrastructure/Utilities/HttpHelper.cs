using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ZsqApp.Core.Infrastructure.Utilities
{
    public class HttpHelper
    {
        public static readonly HttpClient Instance;

        static HttpHelper()
        {
            Instance = new HttpClient();
            Instance.DefaultRequestHeaders.Connection.Add("keep-alive");
            Instance.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36");

            Instance.Timeout = TimeSpan.FromSeconds(60);
        }
    }
}
