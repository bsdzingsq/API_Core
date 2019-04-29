using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Entity.UserEntity;
using AutoMapper;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Models.ZhangYuRequest;

namespace ZsqApp.Core.Services.Data
{
    public class DataService : IData
    {
        /// <summary>
        /// 海南库
        /// </summary>
        private readonly HainanContext _hainanContext;

        /// <summary>
        /// app库
        /// </summary>
        private readonly FunHaiNanContext _funHaiNanContext;

        /// <summary>
        /// 映射
        /// </summary>
        private readonly IMapper _mapper;

       // private readonly IUserTask _userTask;


        public DataService(HainanContext hainanContext, FunHaiNanContext funHaiNanContext, IMapper mapper)
        {
            _hainanContext = hainanContext;
            _funHaiNanContext = funHaiNanContext;
            _mapper = mapper;
        }

        /// <summary>
        /// 拉取老的用户到新库。需要关闭自增
        /// author：陶林辉
        /// </summary>
        public async Task<bool> InsertData()
        {
            /*
             * 注意事项
             * 海南库的用户表需要增加pwd和salt字段，实现要用户的密码和盐值导入到旧表
             * APP库register，login，userInfo除时间和主键外所有字段必须允许为空
             * APP库register在导入数据期间关闭自增列
             * 
             */

            // try
            //  {
            //    var list = await _hainanContext.UserSign.ToListAsync();
            //    UserSignDto signDto = null;
            //    GiveCurrencyLogDto   currencyDto = null;
            //    list.ForEach(m => 
            //    {
            //        signDto = new UserSignDto
            //        {
            //            Multiple=m.Multiple,
            //            Amount=m.amount,
            //            Createtime=m.signTime,
            //            Number=m.userSignNumber,
            //            UserId=m.userId,

            //        };
            //        currencyDto = new GiveCurrencyLogDto
            //        {
            //            CreateTime = m.signTime,
            //            Key = "signIn",
            //            Order = Guid.NewGuid().ToString(),
            //            UserId = m.userId

            //        };
            //        InserGive(currencyDto);
            //        Inser(signDto);
            //    });

            //    return false;
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}


            try
            {
                var list = await _hainanContext.UserTemp.ToListAsync();
                RegisterDto register = null;
                UserLoginDto login = null;
                UserInfoDto userInfo = null;
                list.ForEach(m =>
                {
                    register = new RegisterDto
                    {
                        App_version = m.version,
                        Channel = m.channel_id,
                        Createtime = m.create_time,
                        Device_code = m.device_code,
                        Gps = "",
                        Os_type = m.os_type,
                        Os_version = m.os_version,
                        Phone = m.phone,
                        Platform = "unknown",
                        Updatetime = m.update_time,
                        Userid = m.user_id,
                    };
                    login = new UserLoginDto
                    {
                        Createtime = m.create_time,
                        Is_first = 1,
                        Password = m.password,
                        Phone = m.phone,
                        Salt = m.salt,
                        Status = 0,
                        Token = "",
                        Updatetime = m.update_time,
                        Userid = m.user_id,
                    };
                    userInfo = new UserInfoDto
                    {
                        Createtime = m.create_time,
                        Userid = m.user_id,
                        Updatetime = m.update_time,
                        Head = "",
                        Id_card = m.id_card,
                        Nick_name = m.nick_name,
                        Real_name = m.real_name,

                    };
                    Register(register, login, userInfo);
                });

                return false;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }

        public void Inser(UserSignDto dto)
        {
            var entity = _mapper.Map<UserSignDto, UserSignEntity>(dto);
            _funHaiNanContext.UserSign.Add(entity);
            _funHaiNanContext.SaveChanges();
        }

        public void InserGive(GiveCurrencyLogDto dto)
        {
            var entity = _mapper.Map<GiveCurrencyLogDto, GiveCurrencyLogEntity>(dto);
            _funHaiNanContext.giveCurrencyLog.Add(entity);
            _funHaiNanContext.SaveChanges();
        }
        public void Register(RegisterDto user, UserLoginDto login, UserInfoDto userInfo)
        {
            using (var vTran = _funHaiNanContext.Database.BeginTransaction())
            {
                try
                {
                    var vRegister = _mapper.Map<RegisterDto, RegisterEntity>(user);
                    var vDbRegister = _funHaiNanContext.Register;
                    vDbRegister.Add(vRegister);
                    _funHaiNanContext.SaveChanges();
                    var vLogin = _mapper.Map<UserLoginDto, UserLoginEntity>(login);
                    vLogin.Userid = vRegister.Userid;
                    var vDbLogin = _funHaiNanContext.UserLogin;
                    vDbLogin.Add(vLogin);
                    var vUserInfp = _mapper.Map<UserInfoDto, UserInfoEntity>(userInfo);
                    vUserInfp.Userid = vRegister.Userid;
                    var vDbInfo = _funHaiNanContext.UserInfo;
                    vDbInfo.Add(vUserInfp);
                    _funHaiNanContext.SaveChanges();
                    vTran.Commit(); //提交事务
                                    //return vRegister.Userid;
                }
                catch (global::System.Exception)
                {
                    vTran.Rollback(); //回滚

                    throw;
                }
            }
        }
    }
}
