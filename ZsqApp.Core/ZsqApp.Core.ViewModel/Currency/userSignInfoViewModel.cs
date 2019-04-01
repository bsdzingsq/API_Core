using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.ViewModel.Currency
{
    public class userSignInfoViewModel
    {
        public string UserSignNumber { get; set; }//签到次数
        public DateTime SignTime { get; set; }//签到时间
        public string Amount { get; set; }//金额
        public string Multiple { get; set; }//签到倍数
    }
}
