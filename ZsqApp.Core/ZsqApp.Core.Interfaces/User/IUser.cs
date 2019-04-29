using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.Interfaces.User
{
    //**************操作记录******************
    //创建时间：
    //作者：陶林辉
    //内容描述：用户
    //***************************************
    public interface IUser
    {
        /// <summary>
        /// 初始化用户初始数据
        /// author：陶林辉
        /// </summary>
        /// <param name="user">用户注册数据</param>
        /// <param name="login">用户登陆信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        long Register(RegisterDto user, UserLoginDto login, UserInfoDto userInfo);

        /// <summary>
        /// 判断手机号码是否存在
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        Task<bool> PhoneExistAsync(string phone);

        /// <summary>
        /// 记录用户登陆日志
        /// author：陶林辉
        /// </summary>
        /// <param name="loginLog"></param>
        /// <returns></returns>
        Task RecordLoginLogAsync(userLoginLogDto loginLog);

        /// <summary>
        /// 用户密码登陆
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="pwd">密码</param>
        /// <returns>用户userid</returns>
        long UserLoginOrPwd(string phone, string pwd);

        /// <summary>
        /// 根据用户手机号码获取userid
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns>用户userid</returns>
        long GetUserIdByPhone(string phone);

        /// <summary>
        /// 获取用户信息
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        Task<UserInfoDto> SearchUserInfoAsync(long userId);

        /// <summary>
        /// 设置用户名
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        Task<bool> AlterUserNameAsync(long userId, string name);

        /// <summary>
        /// 修改绑定手机
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">新的手机号码</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        Task<bool> updateBindPhoneAsync(string phone, long userId);

        /// <summary>
        /// 实名认证
        /// author:陶林辉
        /// </summary>
        /// <param name="name">真实姓名</param>
        /// <param name="idCard">身份证号码</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task<bool> updateRealNameAsync(string name, string idCard, long userId);

        /// <summary>
        /// 录取用户反馈
        /// author:陶林辉
        /// </summary>
        /// <param name="feedbackDto">反馈内容</param>
        /// <returns></returns>
        Task<bool> AddFeedbackAsync(FeedbackDto feedbackDto);

        /// <summary>
        /// 判断身份证是否存在
        /// author:陶林辉
        /// </summary>
        /// <param name="val">身份证号码</param>
        /// <returns></returns>
        Task<bool> JudgeIdCard(string val);

        /// <summary>
        /// 找回密码
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        Task<bool> UpdatePwdAsync(string phone, string newPwd);


        /// <summary>
        /// 修改密码
        /// author:陶林辉
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="newPwd">新密码</param>
        /// <param name="salt">盐</param>
        /// <returns></returns>
        Task<bool> UpdatePwdAsync(long userid, string newPwd,int salt);


        /// <summary>
        /// 根据用户id获取用的登陆信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        Task<UserLoginDto> GetUserLoginAsync(long userid);


        /// <summary>
        /// 根据用户手机号获取用的登陆信息
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <returns></returns>
        Task<UserLoginDto> GetUserLoginAsync(string phone);

        /// <summary>
        /// 根据用户id获取用的注册信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        Task<RegisterDto> GetRegisterAsync(long userid);

        /// <summary>
        /// 判断用户id是否存在
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        Task<bool> JudgeUserIdIsNoAsync(long userId);

        /// <summary>
        /// 根据时间和渠道获取用户注册人数
        /// author:白尚德
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        Task<List<RegisterNumberDto>> GetResignCountAsync(DateTime startTime, DateTime ovrtTime);
    }
}
