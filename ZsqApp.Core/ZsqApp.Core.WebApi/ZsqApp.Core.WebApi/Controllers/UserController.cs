using System;
using System.Collections;
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
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.PHPRequest;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.User;
using ZsqApp.Core.WebApi.Filters;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;

namespace ZsqApp.Core.WebApi.Controllers
{
    /// <summary>
    /// 用户相关接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/user")]
    [ActionApiFilterAttribute]
    [EnableCors("any")]
    public class UserController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// 初始化IPhoneMessage
        /// </summary>
        protected readonly IPhoneMessage _mag;
        /// <summary>
        /// 初始化ISystems
        /// </summary>
        protected readonly ISystems _sys;
        /// <summary>
        /// 初始化IUser
        /// </summary>
        protected readonly IUser _user;
        /// <summary>
        /// 初始化IToken
        /// </summary>
        private readonly IToken _token;
        /// <summary>
        /// 初始化IBiz
        /// </summary>
        private readonly IBiz _biz;
        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;

        /// <summary>
        /// 初始化IRoutine
        /// </summary>
        protected readonly IRoutine _routine;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILog _log;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="sys"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="biz"></param>
        /// <param name="accout"></param>
        public UserController(IPhoneMessage mag, ISystems sys, IUser user, IToken token, IRoutine routine, IBiz biz, IAccout accout)
        {
            _mag = mag;
            _sys = sys;
            _user = user;
            _token = token;
            _biz = biz;
            _accout = accout;
            _log = LogManager.GetLogger(Startup.repository.Name, typeof(UserController));
            _routine = routine;
        }
        #endregion

        /// <summary>
        /// 获取验证码
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("sendCode")]
        public async Task<ResponseViewModel<VerifyTokenView>> SendCode([FromBody]RequestViewModel obj)
        {
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<VerifyTokenView> response = null;
            VerifyTokenView result = new VerifyTokenView();
            var sysCode = SysCode.Ok;
            string key = string.Empty;
            string code = string.Empty;
            bool isExist = await _user.PhoneExistAsync((string)obj.Data.phone);
            switch ((string)obj.Data.type)
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
            key = $"{key}{(string)obj.Data.phone}";
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
            //        if (!_mag.MessageCode((string)obj.Data.phone, code, StringExtension.ToInt((string)obj.Data.sendType)))
            //        {
            //            /*获取验证码失败*/
            //            sysCode = SysCode.GetCodeErr;
            //        }
            //        else
            //        {
            //            RedisHelper.StringSet(key, code, 10, RedisFolderEnum.code, RedisEnum.Four);
            //        }
            //    }
            //}
            response = new ResponseViewModel<VerifyTokenView>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 用户注册
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register")]
        public ResponseViewModel<UserTokenView> register([FromBody]RequestViewModel obj)
        {
            UserTokenView token = new UserTokenView();
            ResponseViewModel<UserTokenView> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            string strKey = $"{CacheKey.Rgister.ToString()}{obj.Data.phone}";
            if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) && RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals((string)obj.Data.verifyCode))
            {
                var salt = _sys.GetRandomSeed(8);
                long userId = _user.Register(new RegisterDto
                {
                    App_version = obj.Client.Version,
                    Channel = obj.Client.Channel,
                    Createtime = DateTime.Now,
                    Device_code = obj.Client.DeviceCode,
                    Os_type = obj.Client.OsType,
                    Gps = obj.Client.Gps,
                    Os_version = obj.Client.OsVersion,
                    Phone = obj.Data.phone,
                    Platform = obj.Client.Platform,
                    Updatetime = DateTime.Now
                }, new UserLoginDto
                {
                    Createtime = TimeHelper.GetDateTime(),
                    Is_first = 1,
                    Password = _sys.Md5Encode($"{(string)obj.Data.password}{salt}"),
                    Phone = obj.Data.phone,
                    Status = 0,
                    Updatetime = TimeHelper.GetDateTime(),
                    Salt = salt
                }, new UserInfoDto
                {
                    Createtime = TimeHelper.GetDateTime(),
                    //Head = "",
                    Updatetime = TimeHelper.GetDateTime(),
                    //Nick_name = ""

                });
                if (userId != 0)
                {
                    //获取token
                    token = _token.GetToken(userId);
                    response = new ResponseViewModel<UserTokenView>(SysCode.Ok, token, obj.Encrypt, _sys, obj.Secret);
                    _user.RecordLoginLogAsync(new userLoginLogDto
                    {
                        App_version = obj.Client.Version,
                        Createtime = TimeHelper.GetDateTime(),
                        Device_code = obj.Client.DeviceCode,
                        Gps = obj.Client.Gps,
                        Os_type = obj.Client.OsType,
                        Os_version = obj.Client.OsVersion,
                        Phone = obj.Data.phone,
                        Userid = userId
                    });
                    RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four);
                }
                else
                {
                    response = new ResponseViewModel<UserTokenView>(SysCode.Err, null, obj.Encrypt, _sys, obj.Secret);
                }
            }
            else
            {
                /*不存在验证码，返回验证码错误*/
                response = new ResponseViewModel<UserTokenView>(SysCode.CodeErr, null, obj.Encrypt, _sys, obj.Secret);
            }
            return response;
        }

        /// <summary>
        /// 刷新用户会话令牌
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("refreshToken")]
        public ResponseViewModel<SessionTokenView> RefreshToken([FromBody]RequestViewModel obj)
        {
            var result = new SessionTokenView();
            ResponseViewModel<SessionTokenView> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            string strOpenId = obj.HendInfo.UserOpenId;
            string strToken = (string)obj.Data.token;
            result.SessionToken = _token.GetSessionToken(strOpenId, strToken);
            if (!StringExtension.IsBlank(result.SessionToken))
            {
                response = new ResponseViewModel<SessionTokenView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
            }
            else
            {
                //长令牌失效
                response = new ResponseViewModel<SessionTokenView>(SysCode.TokenLose, null, obj.Encrypt, _sys, obj.Secret);

            }
            return response;
        }

        /// <summary>
        /// 密码登陆
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("pwdLogin")]
        public async Task<ResponseViewModel<UserTokenView>> PwdLogin([FromBody]RequestViewModel obj)
        {
            var result = new UserTokenView();
            ResponseViewModel<UserTokenView> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var code = SysCode.Ok;
            string strPhone = (string)obj.Data.phone;
            bool isExist = await _user.PhoneExistAsync(strPhone);
            if (isExist)
            {
                var userLogin = await _user.GetUserLoginAsync(strPhone);
                string strPwd = _sys.Md5Encode($"{(string)obj.Data.password}{userLogin.Salt}");
                long userId = _user.UserLoginOrPwd(strPhone, strPwd);
                if (userId != 0)
                {
                    result = _token.GetToken(userId);
                    response = new ResponseViewModel<UserTokenView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
                    await _user.RecordLoginLogAsync(new userLoginLogDto
                    {
                        App_version = obj.Client.Version,
                        Createtime = TimeHelper.GetDateTime(),
                        Device_code = obj.Client.DeviceCode,
                        Gps = obj.Client.Gps,
                        Os_type = obj.Client.OsType,
                        Os_version = obj.Client.OsVersion,
                        Phone = obj.Data.phone,
                        Userid = userId
                    });
                }
                else
                {
                    code = SysCode.PwdErr;
                }
            }
            else
            {
                //未注册
                code = SysCode.PhoneNonentity;
            }
            response = new ResponseViewModel<UserTokenView>(code, code == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 验证码登陆
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("codeLogin")]
        public ResponseViewModel<UserTokenView> CodeLogin([FromBody]RequestViewModel obj)
        {
            var result = new UserTokenView();
            ResponseViewModel<UserTokenView> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            string strPhone = (string)obj.Data.phone;
            string strCode = (string)obj.Data.verifyCode;
            string strKey = $"{ CacheKey.LogIn.ToString()}{strPhone}";
            if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) && RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals(strCode))
            {
                long userId = _user.GetUserIdByPhone(strPhone);
                result = _token.GetToken(userId);
                response = new ResponseViewModel<UserTokenView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
                RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four);
            }
            else
            {
                //不存在
                response = new ResponseViewModel<UserTokenView>(SysCode.CodeErr, null, obj.Encrypt, _sys, obj.Secret);
            }
            return response;

        }

        /// <summary>
        /// 获取用户信息
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("qcquireUserInfo")]
        public async Task<ResponseViewModel<UserInfoView>> QcquireUserInfo([FromBody]RequestViewModel obj)
        {
            UserInfoView result = null;
            ResponseViewModel<UserInfoView> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var code = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                UserInfoDto UserInfo = await _user.SearchUserInfoAsync(userLog.Userid);
                UserBalanceDto userBalance = null;
                //关闭章鱼余额查询
                //userBalance = _biz.AcquireBalance(userLog.Userid);
                //从php获取用户余额
                userBalance = _accout.AcquireBalance_php(userLog.Userid);
                _log.InfoFormat("从php获取用户余额:{0},用户信息{1}",JsonHelper.SerializeObject(userBalance),JsonHelper.SerializeObject(UserInfo));
                if (userBalance == null)
                {
                    userBalance = new UserBalanceDto
                    {
                        PrizeBalance = 0.00,
                        Balance = 0.00
                    };
                }
                //查询用户渠道并返回用户信息
                var userchannel = await _routine.GetUserIdChannelIdAsync(userLog.Userid);
                result = new UserInfoView
                {
                    Balance = userBalance.Balance,
                    IdCard = UserInfo.Id_card,
                    Nickname = UserInfo.Nick_name,
                    Phone = userLog.Phone,
                    PrizeBalance = userBalance.PrizeBalance,
                    RealName = UserInfo.Real_name,
                    channel= userchannel
                };

            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<UserInfoView>(code, code == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 资金明细
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("fundRecord")]
        public ResponseViewModel<FundList> FundRecord([FromBody]RequestViewModel obj)
        {
            FundList result = null;
            ResponseViewModel<FundList> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                long userId = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson).Userid;
                //关闭章鱼流水查询
                //   result = _biz.AcquireFund(userId, StringExtension.ToInt((string)obj.Data.type), StringExtension.ToInt((string)obj.Data.pageIndex),
                //StringExtension.ToInt((string)obj.Data.pageSize));
                //从php获取用户流水
                result = _accout.AcquireFund_php(userId, StringExtension.ToInt((string)obj.Data.type), StringExtension.ToInt((string)obj.Data.pageIndex),
             StringExtension.ToInt((string)obj.Data.pageSize));
            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }

            response = new ResponseViewModel<FundList>(code, result != null && result.Page.Count > 0 ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 设置用户名
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("alterUserName")]
        public async Task<ResponseViewModel<object>> AlterUserName([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var vCode = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                isLog = await _user.AlterUserNameAsync(userLog.Userid, (string)obj.Data.name);
                if (!isLog)
                {
                    /*成功*/
                    vCode = SysCode.Err;
                }
            }
            else
            {
                /*短令牌失效*/
                vCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(vCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;

        }

        /// <summary>
        /// 验证申请绑定手机验证码
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("verifyPhoneCode")]
        public ResponseViewModel<VerifyTokenView> VerifyPhoneCode([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<VerifyTokenView> response = null;
            VerifyTokenView result = new VerifyTokenView();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var sysCode = SysCode.Ok;
            if (isLog)
            {
                string code = (string)obj.Data.verifyCode;
                string verifyCode = _sys.Md5Encode(code);
                string key = CacheKey.UnbindPhone.ToString();
                string strLog = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strLog);
                key = $"{key}{userLog.Phone}";
                if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four))
                {

                    if (RedisHelper.StringGet(key, RedisFolderEnum.code, RedisEnum.Four).Equals(code))
                    {
                        RedisHelper.KeyDelete(key, RedisFolderEnum.code, RedisEnum.Four);
                        /*缓存验证码*/
                        key = $"{CacheKey.UnbindPhoneAingle}{userLog.Phone}";
                        result.VerifyToken = verifyCode;
                        RedisHelper.StringSet(key, verifyCode, 10, RedisFolderEnum.code, RedisEnum.Four);

                    }
                    else
                    {
                        /*验证码错误*/
                        sysCode = SysCode.CodeErr;
                    }
                }
                else
                {
                    /*未找到验证码*/
                    sysCode = SysCode.CodeErr;
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<VerifyTokenView>(sysCode, sysCode == SysCode.Ok ? result : null, obj.Encrypt, _sys, obj.Secret);
            return null;
        }

        /// <summary>
        /// 修改绑定绑定手机
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("bindPhone")]
        public async Task<ResponseViewModel<object>> BindPhone([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var sysCode = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                string newPhone = obj.Data.phone;
                string verifyCode = obj.Data.verifyCode;
                string verifyToken = obj.Data.verifyToken;
                string key = $"{CacheKey.BindPhone.ToString()}{newPhone}";
                if (RedisHelper.KeyExists(key, RedisFolderEnum.code, RedisEnum.Four)) //判断新手机验证码是否存在
                {
                    if (RedisHelper.StringGet(key, RedisFolderEnum.code, RedisEnum.Four).Equals(verifyCode))//对比新手机验证码
                    {
                        RedisHelper.KeyDelete(key, RedisFolderEnum.code, RedisEnum.Four);
                        key = $"{CacheKey.UnbindPhoneAingle.ToString()}{userLog.Phone}";
                        if (RedisHelper.StringGet(key, RedisFolderEnum.code, RedisEnum.Four).Equals(verifyToken)) //对比一次性验证码
                        {
                            RedisHelper.KeyDelete(key, RedisFolderEnum.code, RedisEnum.Four);
                            if (await _user.updateBindPhoneAsync(newPhone, userLog.Userid))
                            {
                                /*更新缓存*/
                                userLog.Phone = newPhone;
                                RedisHelper.StringSet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", userLog, RedisFolderEnum.token, RedisEnum.Three);
                            }
                            else
                            {
                                sysCode = SysCode.Err;
                            }

                        }
                        else
                        {
                            /*一次性验证码错误*/
                            sysCode = SysCode.SingleCodeErr;
                        }
                    }
                    else
                    {
                        /*更换手机号验证码错误*/
                        sysCode = SysCode.CodeErr;
                    }

                }
                else
                {
                    /*验证码错误*/
                    sysCode = SysCode.CodeErr;
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;

        }

        /// <summary>
        /// 实名认证
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("enteringRealName")]
        public async Task<ResponseViewModel<object>> EnteringRealName([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var sysCode = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                string realName = obj.Data.realName;
                string idCard = obj.Data.idCard;
                if (!await _user.JudgeIdCard(idCard))
                {
                    if (!await _user.updateRealNameAsync(realName, idCard, userLog.Userid))
                    {
                        sysCode = SysCode.Err;
                    }
                }
                else
                {
                    sysCode = SysCode.IdCardExist;
                }

            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 用户反馈
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("feedback")]
        public async Task<ResponseViewModel<object>> Feedback([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var sysCode = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                if (!await _user.AddFeedbackAsync(new FeedbackDto
                {
                    App_version = obj.Client.Version,
                    Content = obj.Data.opinion,
                    Createtime = DateTime.Now,
                    Updatetime = DateTime.Now,
                    Respond = 0,
                    Userid = userLog.Userid
                }))
                {
                    sysCode = SysCode.Err;
                }
            }
            else
            {
                /*短令牌失效*/
                sysCode = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取用户竞猜列表
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("orderList")]
        public ResponseViewModel<OrderList> OrderList([FromBody]RequestViewModel obj)
        {
            OrderList result = null;
            ResponseViewModel<OrderList> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                long lUserId = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson).Userid;
                result = _biz.AcquireOrder(lUserId, StringExtension.ToInt((string)obj.Data.type), StringExtension.ToInt((string)obj.Data.pageIndex),
                    StringExtension.ToInt((string)obj.Data.pageSize));
            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<OrderList>(code, result != null && result.Page.Count > 0 ? result : null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取用户竞猜投注详情
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("orderDetail")]
        public ResponseViewModel<Order> OrderDetail([FromBody]RequestViewModel obj)
        {
            Order result = new Order();
            ResponseViewModel<Order> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                long lUserId = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson).Userid;
                result = _biz.AcquireOrderDetail((string)obj.Data.forderId);
            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<Order>(code, result, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 找回密码
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("retrievePwd")]
        public async Task<ResponseViewModel<object>> RetrievePwd([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var code = SysCode.Ok;
            string verifyCode = obj.Data.verifyCode;
            string phone = obj.Data.phone;
            string strKey = $"{ CacheKey.SeekPwd.ToString()}{phone}";
            if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) && RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals(verifyCode))
            {
                string pwd = _sys.Md5Encode((string)obj.Data.password);
                if (!await _user.UpdatePwdAsync(phone, pwd))
                {
                    code = SysCode.Err;
                }
                RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four); //删除验证码缓存
            }
            else
            {
                code = SysCode.CodeErr;
            }
            response = new ResponseViewModel<object>(code, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 修改密码
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("updatePwd")]
        public async Task<ResponseViewModel<object>> UpdatePwd([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                var salt = _sys.GetRandomSeed(8);
                string pwd = _sys.Md5Encode($"{(string)obj.Data.password}{salt}");
                string pwdEctype = _sys.Md5Encode($"{(string)obj.Data.password}{salt}");
                if (pwd.Equals(pwdEctype))
                {
                    string verifyCode = obj.Data.verifyCode;
                    string strKey = $"{ CacheKey.UpdatePwd.ToString()}{userLog.Phone}";
                    if (RedisHelper.KeyExists(strKey, RedisFolderEnum.code, RedisEnum.Four) && RedisHelper.StringGet(strKey, RedisFolderEnum.code, RedisEnum.Four).Equals(verifyCode))
                    {
                        if (userLog.Password.Equals(pwd))
                        {
                            code = SysCode.PwdNoDifference; //密码一致
                        }
                        else
                        {
                            if (!await _user.UpdatePwdAsync(userLog.Userid, pwd, salt)) //修改密码
                            {
                                code = SysCode.Err;
                            }
                            RedisHelper.KeyDelete(strKey, RedisFolderEnum.code, RedisEnum.Four); //删除验证码缓存
                        }
                    }
                    else
                    {
                        code = SysCode.CodeErr;
                    }
                }
                else
                {
                    code = SysCode.PwdInconformity;
                }

            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }

            response = new ResponseViewModel<object>(code, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 用户注销
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delToken")]
        public ResponseViewModel<object> DelToken([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                _token.Logout(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<object>(code, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 海峡竞技获取验证码
        /// author：白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("straitSendCode")]
        public async Task<ResponseViewModel<VerifyTokenView>> StraitSendCode([FromBody]RequestViewModel obj)
        {
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<VerifyTokenView> response = null;
            VerifyTokenView result = new VerifyTokenView();
            var sysCode = SysCode.Ok;
            string key = string.Empty;
            string code = string.Empty;
            bool isExist = await _user.PhoneExistAsync((string)obj.Data.phone);
            switch ((string)obj.Data.type)
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
            key = $"{key}{(string)obj.Data.phone}";
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
            //        if (!_mag.StraitMessageCode((string)obj.Data.phone, code, StringExtension.ToInt((string)obj.Data.sendType)))
            //        {
            //            /*获取验证码失败*/
            //            sysCode = SysCode.GetCodeErr;
            //        }
            //        else
            //        {
            //            RedisHelper.StringSet(key, code, 10, RedisFolderEnum.code, RedisEnum.Four);
            //        }
            //    }
            //}
            response = new ResponseViewModel<VerifyTokenView>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }
    }
}