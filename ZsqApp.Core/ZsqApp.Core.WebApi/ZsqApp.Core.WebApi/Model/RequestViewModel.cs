using ZsqApp.Core.Models.Headers;

namespace ZsqApp.Core.WebApi.Model
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：标准请求
    //***************************************
    public class RequestViewModel
    {

        public dynamic Data { get; set; }

        public string Encrypt { get; set; }

        public string Secret { get; set; }

        public HeadConten HendInfo { get; set; }

        public ClientInfo Client { get; set; }

    }
}
