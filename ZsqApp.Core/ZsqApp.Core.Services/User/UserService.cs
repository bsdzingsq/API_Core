using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.UserEntity;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Models.User;

namespace ZsqApp.Core.Services.User
{
    //**************操作记录******************
    //创建时间：2018.1.25
    //作者：陶林辉
    //内容描述：用户相关
    //***************************************
    public class UserService : IUser
    {
        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        public UserService(FunHaiNanContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        /// <summary>
        /// 判断手机号码是否存在
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns></returns>
        public async Task<bool> PhoneExistAsync(string phone)
        {
            return await _context.Register.AnyAsync(m => m.Phone == phone); ;
        }

        /// <summary>
        /// 初始化用户初始数据
        /// author：陶林辉
        /// </summary>
        /// <param name="user">用户注册数据</param>
        /// <param name="login">用户登陆信息</param>
        /// <param name="userInfo">用户信息</param>
        /// <returns>注册用户uerid</returns>
        public long Register(RegisterDto user, UserLoginDto login, UserInfoDto userInfo)
        {
            using (var vTran = _context.Database.BeginTransaction())
            {
                try
                {
                    var vRegister = _mapper.Map<RegisterDto, RegisterEntity>(user);
                    var vDbRegister = _context.Register;
                    vDbRegister.Add(vRegister);
                    _context.SaveChanges();
                    var vLogin = _mapper.Map<UserLoginDto, UserLoginEntity>(login);
                    vLogin.Userid = vRegister.Userid;
                    var vDbLogin = _context.UserLogin;
                    vDbLogin.Add(vLogin);
                    var vUserInfp = _mapper.Map<UserInfoDto, UserInfoEntity>(userInfo);
                    vUserInfp.Userid = vRegister.Userid;
                    var vDbInfo = _context.UserInfo;
                    vDbInfo.Add(vUserInfp);
                    _context.SaveChanges();
                    vTran.Commit(); //提交事务
                    return vRegister.Userid;
                }
                catch (global::System.Exception)
                {
                    vTran.Rollback(); //回滚
                    return 0;
                    throw;
                }
            }
        }

        /// <summary>
        /// 记录用户登陆日志
        /// author：陶林辉
        /// </summary>
        /// <param name="loginLog"></param>
        /// <returns></returns>
        public async Task RecordLoginLogAsync(userLoginLogDto loginLog)
        {
            var vEntity = _mapper.Map<userLoginLogDto, UserLoginLogEntity>(loginLog);
            _context.userLoginLog.Add(vEntity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 用户密码登陆
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="pwd">密码</param>
        /// <returns>用户userid</returns>
        public long UserLoginOrPwd(string phone, string pwd)
        {
            var vInfo = _context.UserLogin.Where(m => m.Phone == phone && m.Password == pwd).FirstOrDefault();
            if (vInfo != null)
            {
                return vInfo.Userid;
            }
            return 0;
        }

        /// <summary>
        /// 根据用户手机号码获取userid
        /// author：陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <returns>用户userid</returns>
        public long GetUserIdByPhone(string phone)
        {
            return _context.UserLogin.Where(m => m.Phone == phone).FirstOrDefault().Userid;

        }

        /// <summary>
        /// 获取用户信息
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<UserInfoDto> SearchUserInfoAsync(long userId)
        {
            var vEntity = await _context.UserInfo.Where(m => m.Userid == userId).FirstOrDefaultAsync();
            return _mapper.Map<UserInfoEntity, UserInfoDto>(vEntity);
        }

        /// <summary>
        /// 设置用户名
        /// author：陶林辉
        /// </summary>
        /// <param name="userId">用户个userid</param>
        /// <param name="name">用户名</param>
        /// <returns></returns>
        public async Task<bool> AlterUserNameAsync(long userId, string name)
        {
            //如果name相等，会修改失败
            var entity = _context.UserInfo.FirstOrDefault(m => m.Userid == userId);
            if (!name.Equals(entity.Nick_name))
            {
                entity.Nick_name = name;
                return await _context.SaveChangesAsync() > 0;

            }
            return true;
        }

        /// <summary>
        /// 修改绑定手机
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">新的手机号码</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<bool> updateBindPhoneAsync(string phone, long userId)
        {
            var register = _context.Register.FirstOrDefault(m => m.Userid == userId);
            var login = _context.UserLogin.FirstOrDefault(m => m.Userid == userId);
            register.Phone = phone;
            login.Phone = phone;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 实名认证
        /// author:陶林辉
        /// </summary>
        /// <param name="name">真实姓名</param>
        /// <param name="idCard">身份证号码</param>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<bool> updateRealNameAsync(string name, string idCard, long userId)
        {
            var entity = _context.UserInfo.FirstOrDefault(m => m.Userid == userId);
            entity.Real_name = name;
            entity.Id_card = idCard;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 判断身份证是否存在
        /// author:陶林辉
        /// </summary>
        /// <param name="val">身份证号码</param>
        /// <returns></returns>
        public async Task<bool> JudgeIdCard(string val)
        {
            return await _context.UserInfo.Where(m => m.Id_card == val).FirstOrDefaultAsync() == null ? false : true;
        }

        /// <summary>
        /// 录取用户反馈
        /// author:陶林辉
        /// </summary>
        /// <param name="feedbackDto">反馈内容</param>
        /// <returns></returns>
        public async Task<bool> AddFeedbackAsync(FeedbackDto feedbackDto)
        {
            var feedEntity = _mapper.Map<FeedbackDto, FeedbackEntity>(feedbackDto);
            _context.Feedback.Add(feedEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 找回密码
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        public async Task<bool> UpdatePwdAsync(string phone, string newPwd)
        {
            var entity = await _context.UserLogin.Where(m => m.Phone == phone).FirstOrDefaultAsync();
            entity.Password = newPwd;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 找回密码
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <param name="newPwd">新密码</param>
        /// <param name="salt">盐</param>
        /// <returns></returns>
        public async Task<bool> UpdatePwdAsync(long userid, string newPwd, int salt)
        {
            var entity = await _context.UserLogin.Where(m => m.Userid == userid).FirstOrDefaultAsync();
            entity.Salt = salt;
            entity.Password = newPwd;
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据用户id获取用的登陆信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public async Task<UserLoginDto> GetUserLoginAsync(long userid)
        {
            var entity = await _context.UserLogin.FirstOrDefaultAsync(m => m.Userid == userid);
            return _mapper.Map<UserLoginEntity, UserLoginDto>(entity);
        }

        /// <summary>
        /// 根据用户手机号获取用的登陆信息
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">用户手机号码</param>
        /// <returns></returns>
        public async Task<UserLoginDto> GetUserLoginAsync(string phone)
        {
            var entity = await _context.UserLogin.FirstOrDefaultAsync(m => m.Phone == phone);
            return _mapper.Map<UserLoginEntity, UserLoginDto>(entity);

        }

        /// <summary>
        /// 根据用户id获取用的注册信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public async Task<RegisterDto> GetRegisterAsync(long userid)
        {
            var entity = await _context.Register.Where(m => m.Userid == userid).FirstOrDefaultAsync();
            return _mapper.Map<RegisterEntity, RegisterDto>(entity);
        }


        /// <summary>
        /// 判断用户id是否存在
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>存在返回true</returns>
        public async Task<bool> JudgeUserIdIsNoAsync(long userId)
        {
            var count = await _context.Register.CountAsync(m => m.Userid == userId);
            return count > 0;
        }

        /// <summary>
        /// 根据时间和渠道获取用户注册人数
        /// author:白尚德
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <returns></returns>
        public async Task<List<RegisterNumberDto>> GetResignCountAsync(DateTime startTime, DateTime ovrtTime)
        {
            var entity = await _context.Register.Where(m => m.Createtime >= startTime && m.Createtime <= ovrtTime).GroupBy(m => m.Channel)
                .Select(item => new RegisterNumberDto
                {
                    num = item.Count(),
                    channel = item.Key
                }).ToListAsync();
            return entity;
        }
        //end
    }

}
