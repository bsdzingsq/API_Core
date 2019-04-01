using System;
using System.Collections;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.Currency;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.User;
using ZsqApp.Core.WebApi.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{
    /// <summary>
    /// 外置投注页面第三期
    /// author:林辉
    /// </summary>
    [Produces("application/json")]
    [EnableCors("any")]
    [Route("api/webbets")]
    public class WebBetsController : Controller
    {

        /// <summary>
        /// 认证
        /// </summary>
        private readonly IToken _token;

        /// <summary>
        /// 章鱼
        /// </summary>
        private readonly IBiz _biz;

        /// <summary>
        /// 游戏key
        /// </summary>
        private readonly GameKeySetting _gameKey;

        /// <summary>
        /// IUser
        /// </summary>
        protected readonly IUser _user;

        /// <summary>
        /// ISystems
        /// </summary>
        protected readonly ISystems _sys;

        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// 用户任务
        /// </summary>
        private readonly IUserTask _userTask;

        /// <summary>
        /// 趣币key配置文件
        /// </summary>
        private readonly CurrencyKeySetting _currencyKey;

        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ZsqApp.Core.WebApi.Controllers.H5Controllres.WebBetsController"/> class.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="biz"></param>
        /// <param name="gameKey"></param>
        /// <param name="user"></param>
        /// <param name="systems"></param>
        /// <param name="userTask"></param>
        /// <param name="currencyKey"></param>
        /// <param name="accout"></param>
        public WebBetsController(IToken token, IBiz biz, IOptions<GameKeySetting> gameKey, IUser user, ISystems systems, IUserTask userTask, IOptions<CurrencyKeySetting> currencyKey, IAccout accout)
        {
            _token = token;
            _biz = biz;
            _gameKey = gameKey.Value;
            _user = user;
            _sys = systems;
            _userTask = userTask;
            _currencyKey = currencyKey.Value;
            _log = _log = LogManager.GetLogger(Startup.repository.Name, typeof(WebBetsController));
            _accout = accout;
        }
        /// <summary>
        /// 外置投注页面获取狗狗or渔网的游戏地址
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameUrl")]
        public async Task<H5ResponseViewModel<GameUrlResponse>> GetUrl([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "gameUrl");
            H5ResponseViewModel<GameUrlResponse> response = null;
            GameUrlResponse result = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isNo = _token.VerifyToken(token);
                if (isNo)
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long user_id = long.Parse(userLogin.Userid.ToString());
                    string temp = Parameters.data.gameId;
                    //获取用户信息，
                    UserInfoDto userInfo = await _user.SearchUserInfoAsync(user_id);
                    string key = string.Empty;
                    switch (temp)
                    {
                        case "1":
                            key = _gameKey.Dog;
                            break;
                        case "2":
                            key = _gameKey.Fish;
                            break;
                        case "3":
                            key = _gameKey.Star;
                            break;
                        default:
                            break;
                    }
                    result = _biz.GameUrl(key, user_id, "H5", 4, "H5", userInfo.Nick_name);
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<GameUrlResponse>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// 新人奖励
        /// author:林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userNewAward")]
        public async Task<H5ResponseViewModel<object>> UserNewAward([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "userverify");
            H5ResponseViewModel<object> response = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isLog = _token.VerifyToken(token);
                if (isLog)
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    sysCode = _userTask.JudgeUser518(userId);
                    bool isNo = false;
                    GiveCurrencyDto giveCurrency = new GiveCurrencyDto
                    {
                        amount = 518.00,
                        forderId = Guid.NewGuid().ToString(),
                        fuserId = userId.ToString(),
                        key = _currencyKey.Activity
                    };
                    if (sysCode == SysCode.Ok)
                    {
                        //关闭章鱼赠币
                       // isNo = await _biz.GiveCurrencyAsync(giveCurrency);
                        //从php赠币
                        isNo = await _accout.GiveCurrencyAsync_php(giveCurrency);
                    }
                    _log.InfoFormat("H5用户518领取， 用户id:{0},订单号:{1},code:{2},赠送结果{3}", userLogin.Userid, giveCurrency.forderId, sysCode, isNo);
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }

            }

            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 查询用户签到信息
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userSignInfo")]
        public async Task<H5ResponseViewModel<UserSignViewModel>> UserSignInfo([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "userverify");
            H5ResponseViewModel<UserSignViewModel> response = null;
            sysCode = SysCode.Ok;
            UserSignViewModel result = new UserSignViewModel
            {
                Multiple = 1,
                Number = 1,
                state = true,
            };
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isLog = _token.VerifyToken(token);
                if (isLog)
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    UserSignDto userSignDto = await _userTask.QueryUserSignAsync(userId);
                    DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//当前时间
                    if (userSignDto != null)
                    {
                        DateTime time = Convert.ToDateTime(userSignDto.Createtime.ToString("yyyy-MM-dd")); //用户最后一次签到时间
                        if (time == nowTime)
                        {
                            result.Multiple = userSignDto.Multiple;
                            result.Number = userSignDto.Number;
                            result.state = false;
                        }
                        else
                        {
                            if (TimeHelper.Dateday(nowTime, Convert.ToDateTime(time)) == 1)
                            {
                                result.Multiple = userSignDto.Multiple;
                                result.Number = userSignDto.Number;
                                result.state = true;
                            }
                        }
                    }
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<UserSignViewModel>(sysCode, result);
            return response;
        }

        /// <summary>
        /// 用户签到
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userSign")]
        public async Task<H5ResponseViewModel<userSignInfoViewModel>> UserSign([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "userverify");
            H5ResponseViewModel<userSignInfoViewModel> response = null;
            userSignInfoViewModel userSignInfo = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isLog = _token.VerifyToken(token);
                if (isLog)
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    UserSignDto userSign = await _userTask.QueryUserSignAsync(userId);
                    UserSignDto userSignDto = null;
                    DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//当前时间
                    Random rd = new Random();
                    int day = 0;
                    decimal multiple = 0.00m;
                    decimal amout = 0.00m;
                    int num = rd.Next(10, 16);//随机数
                    if (userSign != null)
                    {
                        //有签到信息
                        DateTime time = Convert.ToDateTime(userSign.Createtime.ToString("yyyy-MM-dd")); //用户最后一次签到时间
                        if (nowTime != time)
                        {
                            if (TimeHelper.Dateday(nowTime, time) == 1) //当前时间和签到时间相差一天
                            {
                                if (userSign.Number < 6) //小于六天
                                {
                                    multiple = Convert.ToDecimal((userSign.Number * 0.5) + 0.5);//从新计算倍数
                                    amout = num * multiple;
                                }
                                else
                                {
                                    multiple = 3;
                                    amout = num * multiple;
                                }
                                day = userSign.Number + 1;
                            }
                            else
                            {
                                //签到中断，从第一天开始计算
                                multiple = Convert.ToDecimal((1 * 0.5) + 0.5);//签到倍数
                                amout = num * multiple;
                                day = 1;
                            }
                        }
                        else
                        {
                            sysCode = SysCode.RepeatToReceive;
                        }
                    }
                    else
                    {
                        multiple = Convert.ToDecimal((1 * 0.5) + 0.5);//签到倍数
                        amout = num * multiple;
                        day = 1;
                    }
                    if (sysCode == SysCode.Ok)
                    {
                        userSignDto = new UserSignDto
                        {
                            Amount = amout,
                            Createtime = DateTime.Now,
                            Multiple = multiple,
                            Number = day,
                            UserId = userId

                        };
                        if (await _userTask.RecordUserSignAsync(userSignDto)) //记录签到记录
                        {
                            GiveCurrencyDto giveCurrencyDto = new GiveCurrencyDto
                            {
                                amount = double.Parse(userSignDto.Amount.ToString()),
                                forderId = Guid.NewGuid().ToString(),
                                fuserId = userId.ToString(),
                                key = _currencyKey.Sign,
                            };
                            bool isNo = false;
                            //关闭章鱼赠币
                            //isNo = await _biz.GiveCurrencyAsync(giveCurrencyDto);
                            //从php赠币
                            isNo = await _accout.GiveCurrencyAsync_php(giveCurrencyDto);

                            userSignInfo = new userSignInfoViewModel
                            {
                                Amount = amout.ToString(),
                                Multiple = multiple.ToString(),
                                SignTime = DateTime.Now,
                                UserSignNumber = userSignDto.Number.ToString()
                            };
                            _log.InfoFormat("签到,用户id:{0},金额:{1},签到时间{2},订单号:{3},签到结果:{4}"
                                            , userId, userSignDto.Amount, userSignDto.Createtime, giveCurrencyDto.forderId, isNo);
                        }
                    }
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<userSignInfoViewModel>(sysCode, sysCode == SysCode.Ok ? userSignInfo : null);
            return response;
        }

        /// <summary>
        /// 用户反馈
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("webFeedback")]
        public async Task<H5ResponseViewModel<object>> WebFeedback([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "webFeedback");
            H5ResponseViewModel<object> response = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isLog = _token.VerifyToken(token);
                if (isLog)
                {
                    var userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    if (!await _user.AddFeedbackAsync(new FeedbackDto
                    {
                        App_version = "H5",
                        Content = Parameters.data.opinion,
                        Createtime = DateTime.Now,
                        Updatetime = DateTime.Now,
                        Respond = 0,
                        Userid = userId
                    }))
                    {
                        sysCode = SysCode.Err;
                    }
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 获取用户信息
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userInfo")]
        public async Task<H5ResponseViewModel<UserInfoView>> UserInfo([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "userverify");
            UserInfoView Result = null;
            H5ResponseViewModel<UserInfoView> response = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isLog = _token.VerifyToken(token);
                if (isLog)
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    UserInfoDto userInfo = await _user.SearchUserInfoAsync(userId);
                    UserBalanceDto UserBalance = null;
                    //关闭章鱼余额查询
                   // UserBalance = _biz.AcquireBalance(userId);
                    //从php获取用户余额
                    UserBalance = _accout.AcquireBalance_php(userId);

                    if (UserBalance == null)
                    {
                        UserBalance = new UserBalanceDto
                        {
                            PrizeBalance = 0.00,
                            Balance = 0.00
                        };
                    }
                    Result = new UserInfoView
                    {
                        Balance = UserBalance.Balance,
                        IdCard = userInfo.Id_card,
                        Nickname = userInfo.Nick_name,
                        Phone = userLogin.Phone,
                        PrizeBalance = UserBalance.PrizeBalance,
                        RealName = userInfo.Real_name
                    };
                }
                else
                {
                    /*短令牌失效*/
                    sysCode = SysCode.SessionTokenLose;
                }
            }
            response = new H5ResponseViewModel<UserInfoView>(sysCode, Result);
            return response;
        }

        /// <summary>
        /// APP游戏中转页
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameUrl_v1")]
        public async Task<H5ResponseViewModel<GameUrlResponse>> GetUrl_v1([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "gameUrl_v1");
            H5ResponseViewModel<GameUrlResponse> response = null;
            GameUrlResponse result = null;
            if (sysCode == SysCode.Ok)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{Parameters.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                UserInfoDto userInfo = await _user.SearchUserInfoAsync(userLog.Userid);
                string temp = Parameters.data.gameType;
                string key = string.Empty;
                switch (temp)
                {
                    case "3":
                        key = _gameKey.Dog;
                        break;
                    case "2":
                        key = _gameKey.Fish;
                        break;
                    case "4":
                        key = _gameKey.Star;
                        break;
                    default:
                        sysCode = SysCode.GameUrlIsNot;
                        break;
                }
                int osType = 2;
                result = _biz.GameUrl(key, userLog.Userid, "2.0.2", osType, "20002", userInfo.Nick_name);
                if (result == null)
                {
                    sysCode = SysCode.Err;
                }
            }
            response = new H5ResponseViewModel<GameUrlResponse>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// 用户密码登录
        /// author：陶林辉
        /// </summary>
        /// <returns>TokenView</returns>
        /// <param name="Parameters">Parameters.</param>
        [HttpPost]
        [Route("pwdLogin")]
        public async Task<H5ResponseViewModel<TokenView>> PwdLogin([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "PwdLogin_H5");
            H5ResponseViewModel<TokenView> response = null;
            TokenView result = null;
            if (sysCode == SysCode.Ok)
            {
                string phone = Parameters.data.phone;
                //判断手机号码是否注册
                bool isExist = await _user.PhoneExistAsync(phone);
                if (isExist)
                {
                    //获取用户登录信息。
                    var userLogin = await _user.GetUserLoginAsync(phone);
                    if (userLogin.Salt != 0)
                    {
                        string pwd = _sys.Md5Encode($"{(string)Parameters.data.password}{userLogin.Salt}");
                        var userId = _user.UserLoginOrPwd(phone, pwd);
                        if (userId != 0)
                        {
                            result = _token.GetH5Token(userId);
                            await _user.RecordLoginLogAsync(new userLoginLogDto
                            {
                                App_version = "H5",
                                Createtime = TimeHelper.GetDateTime(),
                                Device_code = "H5",
                                Gps = "H5",
                                Os_type = "H5",
                                Os_version = "H5",
                                Phone = phone,
                                Userid = userId
                            });
                        }
                        else
                        {
                            sysCode = SysCode.PwdErr;
                        }
                    }
                    else
                    {
                        //用户H5登录注册，没有密码
                        sysCode = SysCode.UserPwdIsNull;
                    }
                }
                else
                {
                    sysCode = SysCode.PhoneNonentity;
                }
            }
            response = new H5ResponseViewModel<TokenView>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// 修改密码
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatePwd")]
        public async Task<H5ResponseViewModel<object>> UpdatePwd([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<object> response = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "UpdatePwd_H5");
            string token = Parameters.data.token;
            bool isLog = _token.VerifyToken(token);
            if (sysCode == SysCode.Ok)
            {
                if (isLog)
                {
                    var userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>
                                              (RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    var salt = _sys.GetRandomSeed(8);
                    string pwd = _sys.Md5Encode($"{(string)Parameters.data.password}{salt}");
                    string pwdEctype = _sys.Md5Encode($"{(string)Parameters.data.passwordEctype}{salt}");
                    if (pwd.Equals(pwdEctype))
                    {
                        string verifyCode = Parameters.data.verifyCode;
                        string strKey = $"{ CacheKey.UpdatePwd.ToString()}{userLogin.Phone}";
                        if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) &&
                            RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals(verifyCode))
                        {
                            if (userLogin.Password.Equals(pwd))
                            {
                                sysCode = SysCode.PwdNoDifference; //密码一致
                            }
                            else
                            {
                                if (!await _user.UpdatePwdAsync(userLogin.Userid, pwd, salt)) //修改密码
                                {
                                    sysCode = SysCode.Err;
                                }
                                RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four); //删除验证码缓存
                            }
                        }
                        else
                        {
                            sysCode = SysCode.CodeErr;
                        }
                    }
                    else
                    {
                        sysCode = SysCode.PwdInconformity;
                    }
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 设置用户名
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("alterUserName")]
        public async Task<H5ResponseViewModel<object>> AlterUserName([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<object> response = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "alterUserName_H5");
            string token = Parameters.data.token;
            bool isLog = _token.VerifyToken(token);
            if (isLog)
            {
                var userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>
                                             (RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                isLog = await _user.AlterUserNameAsync(userLogin.Userid, (string)Parameters.data.name);
                if (!isLog)
                {
                    sysCode = SysCode.Err;
                }
            }
            else
            {
                sysCode = SysCode.TokenLose;
            }
            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 海峡竞技获取用户信息
        /// author：白尚德
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("userInfoTime")]
        public async Task<H5ResponseViewModel<UserInfoTimeView>> UserInfoTime([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "userInfoTime");
            UserInfoTimeView Result = null;
            H5ResponseViewModel<UserInfoTimeView> response = null;
            if (sysCode == SysCode.Ok)
            {
                string openId = Parameters.data.userOpenId;
                string sessionToken = Parameters.data.sessionToken;
                //token是否有效
                bool isLog = _token.VerifyToken(openId, sessionToken);
                if (isLog)
                {
                    string strJson = RedisHelper.StringGet($"{CacheKey.Token}{openId}", RedisFolderEnum.token, RedisEnum.Three);
                    UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                    Result = new UserInfoTimeView()
                    {
                        Userid = UserLog.Userid,
                        CreateTime = UserLog.Createtime
                    };

                }
                else
                {
                    sysCode = SysCode.SessionTokenLose;
                    Result = null;
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<UserInfoTimeView>(sysCode, Result);
            return response;
        }

    }
}

