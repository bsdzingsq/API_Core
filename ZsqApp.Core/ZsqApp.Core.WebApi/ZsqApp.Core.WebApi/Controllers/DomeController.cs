using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace ZsqApp.Core.WebApi.Controllers
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：测试用
    //***************************************
    [Produces("application/json")]
    [Route("api/Dome")]
    [EnableCors("any")]
    //[ActionApiFilterAttribute]
    //   [GameFilterAttribute]
    public class DomeController
    {
        //private readonly IData _data;
        //private readonly IBiz _biz;
        //private readonly IPhoneMessage _message;
        //public DomeController(IData data, IBiz biz,IPhoneMessage message)
        //{
        //    _data = data;
        //    _biz = biz;
        //    _message = message;
        //}

        /// <summary>
        /// Dome this instance.
        /// </summary>
        [HttpGet]
        [Route("Dome")]
        public void Dome()
        {

        }
    }
}