using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Home;

namespace ZsqApp.Core.Interfaces.Home
{
    public interface IHomeService
    {
        /// <summary>
        /// 海南竞技首页
        /// </summary>
        /// <returns></returns>
        HomePageList homePage();

        /// <summary>
        /// 海南竞技首页2
        /// </summary>
        /// <returns></returns>
        Task<HomePageList> homePage2();

        /// <summary>
        /// 项目介绍
        /// </summary>
        /// <returns></returns>
        Introductionlist Introduction();

        /// <summary>
        /// 首页详情
        /// </summary>
        /// <returns></returns>
        Task<List<HomeRiding>> HomeRidings();

        /// <summary>
        /// 首页游戏
        /// </summary>
        /// <returns></returns>
        Task<List<HomeGames2>> HomeGames();
    }
}
