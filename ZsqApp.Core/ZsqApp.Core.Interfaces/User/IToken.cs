using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.User;

namespace ZsqApp.Core.Interfaces.User
{
    //**************操作记录******************
    //创建时间：
    //作者：陶林辉
    //内容描述：令牌
    //***************************************
    public interface IToken
    {
        /// <summary>
        /// 获取token
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>token信息</returns>
        UserTokenView GetToken(long userId);

        /// <summary>
        /// 认证会话令牌
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外唯一标识</param>
        /// <param name="SessionToken">会话令牌</param>
        /// <returns></returns>
        bool VerifyToken(string userOpenId,string SessionToken);

        /// <summary>
        /// 刷新用户令牌
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外id</param>
        /// <param name="token">用户长令牌</param>
        /// <returns></returns>
        string GetSessionToken(string userOpenId, string token);

        /// <summary>
        /// 用户注销
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外唯一标识</param>
        /// <param name="SessionToken">会话令牌</param>
        /// <returns></returns>
        bool Logout(string userOpenId, string SessionToken);

        /// <summary>
        /// H5获取token
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        TokenView GetH5Token(long userId);

        /// <summary>
        /// 认证会话令牌
        /// author:陶林辉
        /// </summary>
        /// <param name="token">用户长token</param>
        /// <returns></returns>
        bool VerifyToken(string token);
    }
}
