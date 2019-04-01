using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.ViewModel.OfPay
{
    public class MobInfoViewModel
    {
        /// <summary>
        /// 当前用户的手机号码
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 归属地
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string operators { get; set; }

    }
}
