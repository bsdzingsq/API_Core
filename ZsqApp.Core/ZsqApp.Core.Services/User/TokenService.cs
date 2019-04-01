using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.User;

namespace ZsqApp.Core.Services.User
{
    public class TokenService : IToken
    {
        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        private readonly ISystems _sys;
        public TokenService(FunHaiNanContext context, IMapper mapper, ISystems sys)
        {
            _context = context;
            _mapper = mapper;
            _sys = sys;

        }

        /// <summary>
        /// 获取token
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public UserTokenView GetToken(long userId)
        {
            var vInfo = _context.UserLogin.Where(e => e.Userid == userId).FirstOrDefault();
            string strToken = Guid.NewGuid().ToString();
            string strId = _sys.Md5Encode($"{vInfo.Userid}{vInfo.Createtime}");
            string strSession = Guid.NewGuid().ToString();
            var entity = new UserLoginDto
            {
                Createtime=vInfo.Createtime,
                Updatetime=vInfo.Updatetime,
                Is_first=vInfo.Is_first,
                Password=vInfo.Password,
                Phone=vInfo.Phone,
                Salt=vInfo.Salt,
                Status=vInfo.Status,
                Token=strToken,
                Userid=vInfo.Userid
                
            };
            UserTokenView Token = new UserTokenView
            {
                SessionToken = strSession,
                UserOpenId = strId,
                UserToken = strToken
            };
            RedisHelper.KeyDelete($"{CacheKey.Token}{strId}");
            RedisHelper.KeyDelete($"{CacheKey.SessionToken}{strId}");
            RedisHelper.StringSet($"{CacheKey.Token}{strId}", entity, RedisFolderEnum.token, RedisEnum.Three);
            RedisHelper.StringSet($"{CacheKey.SessionToken}{strId}", strSession, 5, RedisFolderEnum.sessionToken, RedisEnum.Three);
            return Token;
        }

        /// <summary>
        /// 验证用户登陆状态
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外唯一Id</param>
        /// <param name="SessionToken">会话Id</param>
        /// <returns></returns>
        public bool VerifyToken(string userOpenId, string SessionToken)
        {
            try
            {
                if (RedisHelper.KeyExists($"{CacheKey.Token}{userOpenId}", RedisFolderEnum.token, RedisEnum.Three))
                {
                    /*短令牌是否存在*/
                    if (RedisHelper.KeyExists($"{CacheKey.SessionToken}{userOpenId}", RedisFolderEnum.sessionToken, RedisEnum.Three))
                    {
                        string strSessionToken = RedisHelper.StringGet($"{CacheKey.SessionToken}{userOpenId}", RedisFolderEnum.sessionToken, RedisEnum.Three);
                        if (strSessionToken.Equals(SessionToken))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// 刷新用户令牌
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外唯一Id</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public string GetSessionToken(string userOpenId, string token)
        {
            string strToken = Guid.NewGuid().ToString();
            if (RedisHelper.KeyExists($"{CacheKey.Token}{userOpenId}", RedisFolderEnum.token, RedisEnum.Three))
            {
                var vToken = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{userOpenId}", RedisFolderEnum.token, RedisEnum.Three));
                if (vToken.Token.Equals(token))
                {
                    RedisHelper.StringSet($"{CacheKey.SessionToken}{userOpenId}", strToken, 5, RedisFolderEnum.sessionToken, RedisEnum.Three);
                    return strToken;
                }
            }
            return "";
        }

        /// <summary>
        /// 用户注销
        /// author:陶林辉
        /// </summary>
        /// <param name="userOpenId">用户对外唯一标识</param>
        /// <param name="SessionToken">会话令牌</param>
        /// <returns></returns>
        public bool Logout(string userOpenId, string SessionToken)
        {
            RedisHelper.KeyDelete($"{CacheKey.Token}{userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
            RedisHelper.KeyDelete($"{CacheKey.SessionToken}{userOpenId}", RedisFolderEnum.sessionToken, RedisEnum.Three);
            return true;
        }

        /// <summary>
        /// H5获取token
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public TokenView GetH5Token(long userId)
        {
            var vInfo = _context.UserLogin.Where(e => e.Userid == userId).FirstOrDefault();
            string token = Guid.NewGuid().ToString();
            RedisHelper.KeyDelete($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five);
            RedisHelper.StringSet($"{CacheKey.Token}{token}", vInfo, 10080, RedisFolderEnum.token, RedisEnum.Five);
            TokenView result = new TokenView
            { token = token };
            vInfo.Token = token;
            _context.SaveChanges();
            return result;
        }

        /// <summary>
        /// 认证会话令牌
        /// author:陶林辉
        /// </summary>
        /// <param name="token">用户长token</param>
        /// <returns></returns>
        public bool VerifyToken(string token)
        {
            try
            {
                if (RedisHelper.KeyExists($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five))
                {
                    UserLoginDto userLogin = JsonHelper.DeserializeJsonToObject<UserLoginDto>(RedisHelper.StringGet($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five));
                    long userId = long.Parse(userLogin.Userid.ToString());
                    var login = _context.UserLogin.Where(e => e.Userid == userId).FirstOrDefault();
                    if (login.Token == token)
                    {
                        return true;
                    }
                    RedisHelper.KeyDelete($"{CacheKey.Token}{token}", RedisFolderEnum.token, RedisEnum.Five);
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}
