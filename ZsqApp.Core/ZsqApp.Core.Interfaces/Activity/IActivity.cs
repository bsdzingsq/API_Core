using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZsqApp.Core.ViewModel.Activity;

namespace ZsqApp.Core.Interfaces.Activity
{
    public interface IActivity
    {
        /// <summary>
        /// 获取活动列表by渠道号
        /// author:陶林辉
        /// </summary>
        /// <returns>The activity list asyn.</returns>
        /// <param name="channel">Channel.</param>
        Task<List<ZsqApp.Core.ViewModel.Activity.Activity>> GetActivityListAsync(string channel);


        /// <summary>
        /// 获取活动列表
        /// author:陶林辉
        /// </summary>
        /// <returns>The activity list asyn.</returns>
        /// <param>.</param>
        Task<List<ZsqApp.Core.ViewModel.Activity.Activity>> GetActivityListAsync();
    }
}
