using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZsqApp.Core.ViewModel.ErrCodeEnum
{
    public enum CacheKey
    {
        /// <summary>
        /// 注册验证码
        /// </summary>
        Rgister,
        /// <summary>
        /// 登陆验证码
        /// </summary>
        LogIn,
        /// <summary>
        /// 找回密码
        /// </summary>
        SeekPwd,
        /// <summary>
        /// 解除绑定手机
        /// </summary>
        UnbindPhone,
        /// <summary>
        /// 解除绑定手机一次性验证码
        /// </summary>
        UnbindPhoneAingle,
        /// <summary>
        /// 绑定手机
        /// </summary>
        BindPhone,
        /// <summary>
        /// OpenId
        /// </summary>
        OpenId,
        /// <summary>
        /// Token
        /// </summary>
        Token,
        /// <summary>
        /// SessionToken
        /// </summary>
        SessionToken,
        /// <summary>
        /// 修改密码
        /// </summary>
        UpdatePwd,
        /// <summary>
        /// H5登陆注册
        /// </summary>
        H5LogIn,

    }
}
