using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.ViewModel.User
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：token返回对象
    //***************************************
    public class UserTokenView
    {
        public  string UserOpenId { get; set; }

        public string UserToken { get; set; }

        public string SessionToken { get; set; }
    }

}
