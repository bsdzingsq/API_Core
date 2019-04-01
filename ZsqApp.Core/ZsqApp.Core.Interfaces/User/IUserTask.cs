using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.Interfaces.User
{
    public interface IUserTask
    {
        /// <summary>
        /// 判断用户是否可以领取518
        /// author:陶林辉
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        SysCode JudgeUser518(long userId);

        /// <summary>
        /// 查询用户签到信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserSignDto> QueryUserSignAsync(long userId);


        /// <summary>
        /// 记录用户签到信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userSign"></param>
        /// <returns></returns>
        Task<bool> RecordUserSignAsync(UserSignDto userSign);
    }
}
