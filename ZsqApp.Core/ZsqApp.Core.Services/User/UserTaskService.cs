using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.Services.User
{
    public class UserTaskService : IUserTask
    {
        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        private readonly CurrencyKeySetting _currencyKey;
        public UserTaskService(FunHaiNanContext context, IMapper mapper, IOptions<CurrencyKeySetting> currencyKey)
        {
            _context = context;
            _mapper = mapper;
            _currencyKey = currencyKey.Value;

        }


        /// <summary>
        /// 判断用户是否可以领取518
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public SysCode JudgeUser518(long userId)
        {
            DateTime time = DateTime.Parse("2017-06-15");
            var entity = _context.Register.Count(m => m.Userid == userId && m.Createtime > time);
            if (entity == 0)
            {
                //查询不到用户，或者用户注册时间不对
                return SysCode.Mismatch;
            }
            var isGet = _context.giveCurrencyLog.Count(m => m.UserId == userId && m.Key == _currencyKey.Activity);
            if (isGet != 0)
            {
                //重复领取
                return SysCode.RepeatToReceive;
            }
            return SysCode.Ok;
        }

        /// <summary>
        /// 查询用户签到信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<UserSignDto> QueryUserSignAsync(long userId)
        {
            var entity = await _context.UserSign.Where(m => m.UserId == userId).OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            return _mapper.Map<UserSignEntity, UserSignDto>(entity);
        }


        /// <summary>
        /// 记录用户签到信息
        /// author:陶林辉
        /// </summary>
        /// <param name="userSign"></param>
        /// <returns></returns>
        public async Task<bool> RecordUserSignAsync(UserSignDto userSign)
        {
            var entity = _mapper.Map<UserSignDto, UserSignEntity>(userSign);
            _context.UserSign.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
