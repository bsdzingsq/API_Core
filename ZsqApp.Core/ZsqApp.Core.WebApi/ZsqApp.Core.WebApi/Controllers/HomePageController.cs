using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZsqApp.Core.Interfaces.Home;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models.Home;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;

namespace ZsqApp.Core.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/HomePage")]
    public class HomePageController : BaseController
    {
        /// <summary>
        /// ISystems
        /// </summary>
        protected readonly ISystems _sys;
        private readonly IHomeService _homeservice;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="systems"></param>
        /// <param name="homeservice"></param>
        public HomePageController(ISystems systems, IHomeService homeservice)
        {
            _sys = systems;
            _homeservice = homeservice;
        }

        /// <summary>
        /// 海南竞技首页
        /// author:白尚德
        /// </summary>
        [HttpPost]
        [Route("homepage2")]
        public H5ResponseViewModel<HomePageList> HomePage2([FromBody]RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.Data, "homepage");
            HomePageList homePage = null;
            H5ResponseViewModel<HomePageList> response = null;
            if (sysCode == SysCode.Ok)
            {
                var strjson = _homeservice.homePage();
                homePage = strjson;

            }
            response = new H5ResponseViewModel<HomePageList>(sysCode, homePage ?? null);
            return response;
        }

        /// <summary>
        /// 海南竞技首页
        /// author:白尚德
        /// </summary>
        [HttpPost]
        [Route("homepage")]
        public async Task<H5ResponseViewModel<HomePageList>> HomePage([FromBody]RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.Data, "homepage");
            HomePageList homePage = null;
            H5ResponseViewModel<HomePageList> response = null;
            if (sysCode == SysCode.Ok)
            {
                var strjson = await _homeservice.homePage2();
                homePage = strjson;

            }
            response = new H5ResponseViewModel<HomePageList>(sysCode, homePage ?? null);
            return response;
        }

        /// <summary>
        /// 海南竞技介绍
        /// author:白尚德
        /// </summary>
        [HttpPost]
        [Route("recommend")]
        public H5ResponseViewModel<Introductionlist> Recommend([FromBody]RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.Data, "recommend");
            Introductionlist introduce = null;
            H5ResponseViewModel<Introductionlist> response = null;
            if (sysCode == SysCode.Ok)
            {
                var strjson = _homeservice.Introduction();
                introduce = strjson;

            }
            response = new H5ResponseViewModel<Introductionlist>(sysCode, introduce ?? null);
            return response;
        }

    }
}