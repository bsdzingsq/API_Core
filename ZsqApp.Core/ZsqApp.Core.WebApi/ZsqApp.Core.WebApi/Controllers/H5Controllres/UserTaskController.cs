using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.Message;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.PHPRequest;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.Currency;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.User;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{
    /// <summary>
    /// H5提供用户相关接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/UserTask")]
    [EnableCors("any")]
    public class UserTaskController : Controller
    {
        #region dependency injection
        /// <summary>
        /// 认证
        /// </summary>
        private readonly IToken _token;
        /// <summary>
        /// 用户任务
        /// </summary>
        private readonly IUserTask _userTask;
        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;
        /// <summary>
        /// 章鱼
        /// </summary>
        private readonly IBiz _biz;
        /// <summary>
        /// 用户
        /// </summary>
        protected readonly IUser _user;
        /// <summary>
        /// 系统
        /// </summary>
        protected readonly ISystems _sys;
        /// <summary>
        /// 短信语音推送
        /// </summary>
        protected readonly IPhoneMessage _msg;

        /// <summary>
        /// 趣币key配置文件
        /// </summary>
        private readonly CurrencyKeySetting _currencyKey;

        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userTask"></param>
        /// <param name="biz"></param>
        /// <param name="user"></param>
        /// <param name="sys"></param>
        /// <param name="msg"></param>
        /// <param name="currencyKey"></param>
        /// <param name="accout"></param>
        public UserTaskController(IToken token, IUserTask userTask, IBiz biz, IUser user, ISystems sys, IPhoneMessage msg, IOptions<CurrencyKeySetting> currencyKey, IAccout accout)
        {
            _token = token;
            _userTask = userTask;
            _log = _log = LogManager.GetLogger(Startup.repository.Name, typeof(OfpayController));
            _biz = biz;
            _user = user;
            _sys = sys;
            _msg = msg;
            _currencyKey = currencyKey.Value;
            _accout = accout;
        }
        #endregion

        /// <summary>
        /// 用户领取518
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("usereight")]
        public async Task<H5ResponseViewModel<object>> UserEight([FromBody]H5RequestViewModel Parameters)
        {
            Thread.Sleep(3000);
            H5ResponseViewModel<object> response = null;
            bool isLog = _token.VerifyToken((string)Parameters.data.userOpenId, (string)Parameters.data.sessionToken);
            var sysCode = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{(string)Parameters.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                sysCode = _userTask.JudgeUser518(userLog.Userid);
                GiveCurrencyDto giveCurrency = new GiveCurrencyDto
                {
                    amount = 518.00,
                    forderId = Guid.NewGuid().ToString(),
                    fuserId = userLog.Userid.ToString(),
                    key = _currencyKey.Activity
                };
                bool isNo = false;
                if (sysCode == SysCode.Ok)
                {
                    //关闭章鱼赠币
                    //isNo = await _biz.GiveCurrencyAsync(giveCurrency);
                    //从php赠币
                    isNo = await _accout.GiveCurrencyAsync_php(giveCurrency);
                }
                _log.InfoFormat("518，用户id:{0},订单号:{1},code:{2},赠送结果{3}", userLog.Userid, giveCurrency.forderId, sysCode, isNo);
            }
            else
            {
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 用户签到
        /// author：陶林辉
        /// 根据之前的签到逻辑抄的。
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("usersign")]
        public async Task<H5ResponseViewModel<userSignInfoViewModel>> UserSign([FromBody]H5RequestViewModel Parameters)
        {
            Thread.Sleep(3000);
            H5ResponseViewModel<userSignInfoViewModel> response = null;
            bool isLog = _token.VerifyToken((string)Parameters.data.userOpenId, (string)Parameters.data.sessionToken);
            var sysCode = SysCode.Ok;
            userSignInfoViewModel userSignInfo = null;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{(string)Parameters.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                var userSign = await _userTask.QueryUserSignAsync(userLog.Userid);
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
                        UserId = userLog.Userid

                    };
                    if (await _userTask.RecordUserSignAsync(userSignDto)) //记录签到记录
                    {
                        GiveCurrencyDto giveCurrencyDto = new GiveCurrencyDto
                        {
                            amount = double.Parse(userSignDto.Amount.ToString()),
                            forderId = Guid.NewGuid().ToString(),
                            fuserId = userLog.Userid.ToString(),
                            key = _currencyKey.Sign,
                        };
                        bool isNo = false;
                        //关闭章鱼赠币
                        //isNo = await _biz.GiveCurrencyAsync(giveCurrencyDto);
                        //从php 赠币
                        isNo = await _accout.GiveCurrencyAsync_php(giveCurrencyDto);

                        userSignInfo = new userSignInfoViewModel
                        {
                            Amount = amout.ToString(),
                            Multiple = multiple.ToString(),
                            SignTime = DateTime.Now,
                            UserSignNumber = userSignDto.Number.ToString()
                        };
                        _log.InfoFormat("签到,用户id:{0},金额:{1},签到时间{2},订单号:{3},签到结果:{4}"
                            , userLog.Userid, userSignDto.Amount, userSignDto.Createtime, giveCurrencyDto.forderId, isNo);
                    }
                }
            }
            else
            {
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<userSignInfoViewModel>(sysCode, sysCode == SysCode.Ok ? userSignInfo : null);
            return response;
        }

        /// <summary>
        /// 用户签到
        /// author：陶林辉
        /// 根据之前的签到逻辑抄的。
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("usersign_v1")]
        public async Task<H5ResponseViewModel<userSignInfoViewModel>> UserSign_v1([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<userSignInfoViewModel> response = null;
            string userId = _biz.IsNoLogin((string)Parameters.data.token, (string)Parameters.data.userOpenId);
            var sysCode = SysCode.Ok;
            userSignInfoViewModel userSignInfo = null;
            if (userId != "0")
            {
                int day = 0;
                decimal multiple = 0.00m;
                decimal amout = 0.00m;
                long user_id = long.Parse(userId);
                var userSign = await _userTask.QueryUserSignAsync(user_id);
                UserSignDto userSignDto = null;
                DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//当前时间
                Random rd = new Random();
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
                        UserId = long.Parse(userId)

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
                        //从php 赠币
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
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<userSignInfoViewModel>(sysCode, sysCode == SysCode.Ok ? userSignInfo : null);
            return response;
        }

        /// <summary>
        /// 查询用户签到信息
        /// author：陶林辉
        /// 根据之前的签到逻辑抄的。
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("usersigninfo_v1")]
        public async Task<H5ResponseViewModel<UserSignViewModel>> UserSignInfo_v1([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<UserSignViewModel> response = null;
            string userId = _biz.IsNoLogin((string)Parameters.data.token, (string)Parameters.data.userOpenId);
            var sysCode = SysCode.Ok;
            UserSignViewModel result = new UserSignViewModel
            {
                Multiple = 1,
                Number = 1,
                state = true,
            };
            if (userId != "0")
            {
                long user_id = long.Parse(userId);
                var userSignDto = await _userTask.QueryUserSignAsync(user_id);
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
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<UserSignViewModel>(sysCode, result);
            return response;
        }

        /// <summary>
        /// 查询用户签到信息
        /// author：陶林辉
        /// 根据之前的签到逻辑抄的。
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("usersigninfo")]
        public async Task<H5ResponseViewModel<UserSignViewModel>> UserSignInfo([FromBody]H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<UserSignViewModel> response = null;
            bool isLog = _token.VerifyToken((string)Parameters.data.userOpenId, (string)Parameters.data.sessionToken);
            var sysCode = SysCode.Ok;
            UserSignViewModel result = new UserSignViewModel
            {
                Multiple = 1,
                Number = 1,
                state = true,
            };
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{(string)Parameters.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                var userSignDto = await _userTask.QueryUserSignAsync(userLog.Userid);
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
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<UserSignViewModel>(sysCode, result);
            return response;
        }

        /// <summary>
        /// 章鱼版获取四星游戏地址
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("geturl")]
        public H5ResponseViewModel<GameUrlResponse> GetUrl([FromBody]H5RequestViewModel Parameters)
        {

            H5ResponseViewModel<GameUrlResponse> response = null;
            GameUrlResponse result = null;
            var sysCode = SysCode.Ok;
            string userId = _biz.IsNoLogin((string)Parameters.data.token, (string)Parameters.data.userOpenId);
            if (userId != "0")
            {
                long user_id = long.Parse(userId);
                result = _biz.GameUrl("sixc", user_id, "", 1, "", "");
            }
            else
            {
                sysCode = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<GameUrlResponse>(sysCode, result);
            return response;
        }


        /// <summary>
        /// H5获取短信验证码
        /// authot:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendCode")]
        public async Task<H5ResponseViewModel<Object>> SendCode([FromBody]H5RequestViewModel Parameters)
        {

            H5ResponseViewModel<Object> response = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "SendCode"); ;
            string phone = Parameters.data.phone;
            bool isExist = await _user.PhoneExistAsync(phone);
            if (sysCode == SysCode.Ok)
            {
                string key = string.Empty;
                string code = string.Empty;
                switch ((string)Parameters.data.type)
                {
                    case "1":
                        key = CacheKey.Rgister.ToString();
                        if (isExist)
                        {
                            /*手机号码已经注册*/
                            sysCode = SysCode.PhoneExist;
                        }
                        break;
                    case "2":
                        key = CacheKey.LogIn.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    case "3":
                        key = CacheKey.SeekPwd.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    case "4":
                        key = CacheKey.UnbindPhone.ToString();
                        break;
                    case "5":
                        key = CacheKey.BindPhone.ToString();
                        if (isExist)
                        {
                            /*手机号码已经注册*/
                            sysCode = SysCode.PhoneExist;
                        }
                        break;
                    case "6":
                        key = CacheKey.UpdatePwd.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    default:
                        break;
                }
                key = $"{key}{(string)Parameters.data.phone}";
                #region
                code = "123456"; //_sys.getrandomseed(4).tostring();
                if (sysCode == SysCode.Ok)
                {
                    if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //是否存在
                    {
                        /*重复获取*/
                        sysCode = SysCode.RepeatedGetCode;
                    }
                    if (sysCode == SysCode.Ok)
                    {

                        //!_mag.messagecode((string)obj.data.phone, code, stringextension.toint((string)obj.data.sendtype))
                        if (false)
                        {
                            /*获取验证码失败*/
                            sysCode = SysCode.GetCodeErr;
                        }
                        else
                        {
                            RedisHelper.StringSet(key, code, 10, RedisFolderEnum.code, RedisEnum.Four);
                        }
                    }
                }
                #endregion
                //code = _sys.GetRandomSeed(4).ToString();
                //if (sysCode == SysCode.Ok)
                //{
                //    if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //是否存在
                //    {
                //        /*重复获取*/
                //        sysCode = SysCode.RepeatedGetCode;
                //    }
                //    if (sysCode == SysCode.Ok)
                //    {
                //        if (!_msg.MessageCode((string)Parameters.data.phone, code, ((string)Parameters.data.sendType).ToInt()))
                //        {
                //            /*获取验证码失败*/
                //            sysCode = SysCode.GetCodeErr;
                //        }
                //        else
                //        {
                //            _log.Info("获取短信验证码成功");
                //            RedisHelper.StringSet(key, code, 1, RedisFolderEnum.code, RedisEnum.Four);
                //        }
                //    }
                //}
            }
            response = new H5ResponseViewModel<Object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// H5注册
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public H5ResponseViewModel<Object> Register([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "Register");
            H5ResponseViewModel<Object> response = null;
            string phone = Parameters.data.phone;
            string strKey = $"{CacheKey.Rgister.ToString()}{phone}";
            if (sysCode == SysCode.Ok)
            {
                if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four)
                    && RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals((string)Parameters.data.verifyCode))
                {
                    var salt = _sys.GetRandomSeed(8);
                    long userId = _user.Register(new RegisterDto
                    {
                        App_version = "",
                        Channel = Parameters.data.channelId,
                        Createtime = DateTime.Now,
                        Device_code = "",
                        Os_type = "",
                        Gps = "",
                        Os_version = "",
                        Phone = phone,
                        Platform = "H5",
                        Updatetime = DateTime.Now
                    }, new UserLoginDto
                    {
                        Createtime = TimeHelper.GetDateTime(),
                        Is_first = 1,
                        Password = _sys.Md5Encode($"{(string)Parameters.data.password}{salt}"),
                        Phone = phone,
                        Status = 0,
                        Salt = salt,
                        Updatetime = TimeHelper.GetDateTime(),
                    }, new UserInfoDto
                    {
                        Createtime = TimeHelper.GetDateTime(),
                        Head = "",
                        Updatetime = TimeHelper.GetDateTime(),
                        Nick_name = ""

                    });
                    if (userId != 0)
                    {
                        RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four);
                    }
                    else
                    {
                        sysCode = SysCode.Err;
                    }
                }
                else
                {
                    sysCode = SysCode.CodeErr;
                }
            }
            response = new H5ResponseViewModel<Object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// H5登陆注册获取验证码
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("loginsendcode")]
        public H5ResponseViewModel<Object> LoginSendCode([FromBody] H5RequestViewModel Parameters)
        {
            H5ResponseViewModel<Object> response = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "LoginSendCode"); ;
            string phone = Parameters.data.phone;
            string key = $"{CacheKey.H5LogIn.ToString()}{phone}";
            if (sysCode == SysCode.Ok)
            {
                if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //是否存在
                {
                    /*重复获取*/
                    sysCode = SysCode.RepeatedGetCode;
                }
                if (sysCode == SysCode.Ok)
                {
                    #region
                    string code = "123456";
                    if (false)
                    {
                        /*获取验证码失败*/
                        sysCode = SysCode.GetCodeErr;
                    }
                    else
                    {
                        RedisHelper.StringSet(key, code, 1, RedisFolderEnum.code, RedisEnum.Four);
                    }
                    #endregion
                    //string code = _sys.GetRandomSeed(4).ToString();
                    //if (!_msg.MessageCode((string)Parameters.data.phone, code, StringExtension.ToInt((string)Parameters.data.sendType)))
                    //{
                    //    /*获取验证码失败*/
                    //    sysCode = SysCode.GetCodeErr;
                    //}
                    //else
                    //{
                    //    RedisHelper.StringSet(key, code, 1, RedisFolderEnum.code, RedisEnum.Four);
                    //}
                }
            }
            response = new H5ResponseViewModel<object>(sysCode, null);
            return response;
        }

        /// <summary>
        /// 外置的登陆注册接口
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        public async Task<H5ResponseViewModel<TokenView>> Login([FromBody]H5RequestViewModel Parameters)
        {
            //判断参数完整
            var sysCode = _sys.CheckParameters(Parameters.data, "Login");
            H5ResponseViewModel<TokenView> response = null;
            TokenView result = null;
            //用户手机号码
            string phone = Parameters.data.phone;
            //验证码缓存Key
            string strKey = $"{CacheKey.H5LogIn.ToString()}{phone}";
            if (sysCode == SysCode.Ok)
            {
                //判断验证码是否正确
                if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) &&
                    RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals((string)Parameters.data.verifyCode))
                {
                    //判断手机号码是否注册
                    long userId = 0;
                    if (await _user.PhoneExistAsync(phone))
                    {
                        userId = _user.GetUserIdByPhone(phone);
                    }
                    else
                    {
                        /*注册信息*/
                        /*登陆信息*/
                        /*用户基本信息*/
                        userId = _user.Register(new RegisterDto
                        {
                            App_version = "",
                            Channel = Parameters.data.channelId,
                            Createtime = DateTime.Now,
                            Device_code = "",
                            Os_type = "",
                            Gps = "",
                            Os_version = "",
                            Phone = phone,
                            Platform = "H5",
                            Updatetime = DateTime.Now
                        }, new UserLoginDto
                        {
                            Createtime = TimeHelper.GetDateTime(),
                            Is_first = 1,
                            Password = "",
                            Phone = phone,
                            Status = 0,
                            Salt = 0,
                            Updatetime = TimeHelper.GetDateTime(),
                        }, new UserInfoDto
                        {
                            Createtime = TimeHelper.GetDateTime(),
                            Head = "",
                            Updatetime = TimeHelper.GetDateTime(),
                            Nick_name = ""

                        });
                    }
                    if (userId != 0)
                    {
                        result = _token.GetH5Token(userId);
                        //记录用户登陆日志
                        userLoginLogDto Log = new userLoginLogDto
                        {
                            App_version = "H5",
                            Createtime = TimeHelper.GetDateTime(),
                            Device_code = "H5",
                            Gps = "H5",
                            Os_type = "H5",
                            Os_version = "H5",
                            Phone = phone,
                            Userid = userId
                        };
                        await _user.RecordLoginLogAsync(Log);
                    }
                    else
                    {
                        sysCode = SysCode.Err;
                    }
                }
                else
                {
                    //验证码不存在或者验证错误
                    sysCode = SysCode.CodeErr;
                }
            }
            response = new H5ResponseViewModel<TokenView>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// H5获取用户流水
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("fundrecord")]
        public H5ResponseViewModel<FundList> FundRecord([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "FundRecord_H5");
            H5ResponseViewModel<FundList> response = null;
            FundList result = null;
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                //token是否有效
                bool isNo = _token.VerifyToken(token);
                if (isNo)
                {
                    //读取redis用户基本信息
                    var userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    //交易类型
                    int type = ((string)Parameters.data.type).ToInt();
                    //请求分页页码
                    int pageIndex = ((string)Parameters.data.pageIndex).ToInt();
                    //请求分页的大小
                    int pageSize = ((string)Parameters.data.pageSize).ToInt();
                    //关闭章鱼流水查询
                    //查询用户流水
                    //result = _biz.AcquireFund(userLogin.Userid, type, pageIndex, pageSize);
                    //从php查询用户流水
                    result = _accout.AcquireFund_php(userLogin.Userid, type, pageIndex, pageSize);

                }
                else
                {
                    //token失效
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<FundList>(sysCode, result ?? null);
            return response;
        }

        /// <summary>
        /// 根据商城订单号查询订单状态
        /// author:陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("costStatus")]
        public H5ResponseViewModel<CostStatusResult> CostStatus([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "costStatus");
            H5ResponseViewModel<CostStatusResult> response = null;
            CostStatusResult result = null;
            CostStatusResult result2 = null;
            CostStatusResult result3 = null;
            if (sysCode == SysCode.Ok)
            {
                //result = _biz.CostStatus((string)Parameters.data.forderId);
                result = _accout.CostStatus_php((string)Parameters.data.forderId);

                result2 = _accout.CostOrderStatus_php((string)Parameters.data.forderId);
               

                if (result == null && result2 == null)
                {
                    result3 = null;
                }
                else
                {
                    result3 = (result == null) ? result2 : result;
                }

                //从php根据兑出订单号查询订单状态
                // _accout.CostStatus_php((string)Parameters.data.forderId);
            }
            response = new H5ResponseViewModel<CostStatusResult>(sysCode, result3 ?? null);
            return response;
        }

        /// <summary>
        /// H5登陆注册获取验证码
        /// author:白尚德
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("verificationCode")]
        public async Task<H5ResponseViewModel<Object>> VerificationCode([FromBody]H5RequestViewModel Parameters)
        {

            H5ResponseViewModel<Object> response = null;
            var sysCode = _sys.CheckParameters(Parameters.data, "verificationCode");
            string phone = Parameters.data.phone;
            bool isExist = await _user.PhoneExistAsync(phone);
            if (sysCode == SysCode.Ok)
            {
                string key = string.Empty;
                string code = string.Empty;
                switch ((string)Parameters.data.type)
                {
                    case "1":
                        key = CacheKey.Rgister.ToString();
                        if (isExist)
                        {
                            /*手机号码已经注册*/
                            sysCode = SysCode.PhoneExist;
                        }
                        break;
                    case "2":
                        key = CacheKey.LogIn.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    case "3":
                        key = CacheKey.SeekPwd.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    case "4":
                        key = CacheKey.UnbindPhone.ToString();
                        break;
                    case "5":
                        key = CacheKey.BindPhone.ToString();
                        if (isExist)
                        {
                            /*手机号码已经注册*/
                            sysCode = SysCode.PhoneExist;
                        }
                        break;
                    case "6":
                        key = CacheKey.UpdatePwd.ToString();
                        if (!isExist)
                        {
                            /*未注册*/
                            sysCode = SysCode.PhoneNonentity;
                        }
                        break;
                    default:
                        break;
                }
                key = $"{key}{(string)Parameters.data.phone}";
                #region
                code = "123456"; //_sys.getrandomseed(4).tostring();
                if (sysCode == SysCode.Ok)
                {
                    if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //是否存在
                    {
                        /*重复获取*/
                        sysCode = SysCode.RepeatedGetCode;
                    }
                    if (sysCode == SysCode.Ok)
                    {

                        //!_mag.messagecode((string)obj.data.phone, code, stringextension.toint((string)obj.data.sendtype))
                        if (false)
                        {
                            /*获取验证码失败*/
                            sysCode = SysCode.GetCodeErr;
                        }
                        else
                        {
                            RedisHelper.StringSet(key, code, 10, RedisFolderEnum.code, RedisEnum.Four);
                        }
                    }
                }
                #endregion
                //code = _sys.GetRandomSeed(4).ToString();
                //if (sysCode == SysCode.Ok)
                //{
                //    if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //是否存在
                //    {
                //        /*重复获取*/
                //        sysCode = SysCode.RepeatedGetCode;
                //    }
                //    if (sysCode == SysCode.Ok)
                //    {
                //        if (!_msg.StraitMessageCode((string)Parameters.data.phone, code, ((string)Parameters.data.sendType).ToInt()))
                //        {
                //            /*获取验证码失败*/
                //            sysCode = SysCode.GetCodeErr;
                //        }
                //        else
                //        {
                //            _log.Info("获取短信验证码成功");
                //            RedisHelper.StringSet(key, code, 1, RedisFolderEnum.code, RedisEnum.Four);
                //        }
                //    }
                //}
            }
            response = new H5ResponseViewModel<Object>(sysCode, null);
            return response;
        }
    }
}