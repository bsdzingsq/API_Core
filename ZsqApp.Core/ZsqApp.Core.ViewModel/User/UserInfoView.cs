using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.ViewModel.User
{
    public class UserInfoView
    {
        public double Balance { get; set; }

        public double PrizeBalance { get; set; }

        public string Phone { get; set; }

        public string Nickname { get; set; }

        public string RealName { get; set; }

        public string IdCard { get; set; }

        public DateTime CreateTime { get; set; }

        public string channel { get; set; }
    }
}
