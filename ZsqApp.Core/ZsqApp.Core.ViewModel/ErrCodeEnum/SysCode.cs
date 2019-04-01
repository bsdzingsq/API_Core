using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ZsqApp.Core.ViewModel.ErrCodeEnum
{
    public enum SysCode
    {
        [Description("成功")]
        Ok = 0,

        [Description("失败")]
        Err = -1,

        [Description("appKey不存在")]
        AppKey = -2,

        [Description("非法时间戳")]
        TimestampErr = -3,

        [Description("参数不完整")]
        LackParameter = -4,

        [Description("必要参数为空")]
        EmptyParameter = -5,

        [Description("参数格式错误")]
        ErrParameter = -6,

        [Description("签名校验失败")]
        SignCheckErr = -7,

        [Description("Base64解码失败")]
        Base64DecodeErr = -8,


        [Description("应用版本不可用或版本不存在")]
        VersionIsNo = -9,

        [Description("未知错误")]
        Unknown = 9999,

        [Description("重复获取验证码")]
        RepeatedGetCode = 1001,

        [Description("获取验证码失败")]
        GetCodeErr = 1002,

        [Description("验证码错误")]
        CodeErr = 1003,

        [Description("手机已注册")]
        PhoneExist = 1004,

        [Description("手机未注册")]
        PhoneNonentity = 1005,

        [Description("SessionToken失效")]
        SessionTokenLose = 1006,

        [Description("Token失效")]
        TokenLose = 1007,

        [Description("密码错误")]
        PwdErr = 1008,

        [Description("一次性验证码错误")]
        SingleCodeErr = 1009,

        [Description("身份证信息存在")]
        IdCardExist = 1010,

        [Description("充值商品不存在或已下架")]
        RechargeCommodityIsNULL = 1011,

        [Description("两次密码不一致")]
        PwdInconformity = 1012,

        [Description("密码和原密码一致")]
        PwdNoDifference = 1013,

        [Description("openId错误")]
        UserOpenIdisNo = 1014,

        [Description("用户不存在")]
        UserExist = 1015,

        [Description("手机号码格式错误")]
        PhoneFormatErr = 1016,

        [Description("密码格式错误")]
        PwdFormatErr = 1017,

        [Description("订单号以存在")]
        OrderExist = 1018,

        [Description("渠道号格式错误")]
        ChannelErr =1019,

        [Description("订单重复充值")]
        OrderIsSuccess = 1020,

        [Description("订单充值中")]
        OrderIsimplement = 1021,

        [Description("用户密码为空")]
        UserPwdIsNull=1022,

        [Description("不符合领取条件")]
        Mismatch = 2000,

        [Description("重复领取")]
        RepeatToReceive = 2001,

        [Description("查询不到当前数据")]
        IdIsNull = 2002,

        [Description("游戏暂时不支持获取地址")]
        GameUrlIsNot = 2003,

        [Description("用户不一致")]
        UserIsNo = 2004
    }
}
