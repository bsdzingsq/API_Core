using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.OfPay;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Ofpay;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.OfPay;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{

    /// <summary>
    /// H5欧飞相关
    /// </summary>
    [Produces("application/json")]
    [Route("api/Ofpay")]
    [EnableCors("any")]
    public class OfpayController : Controller
    {
        #region dependency injection
        /// <summary>
        /// 欧飞
        /// </summary>
        private readonly IOfpay _ofpay;
        /// <summary>
        /// 用户认证
        /// </summary>
        private readonly IToken _token;
        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;
        /// <summary>
        /// 章鱼
        /// </summary>
        private readonly IBiz _biz;
        /// <summary>
        /// php
        /// </summary>
        private readonly IAccout _accout;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="ofpay"></param>
        /// <param name="token"></param>
        /// <param name="biz"></param>
        /// <param name="accout"></param>
        public OfpayController(IOfpay ofpay, IToken token, IBiz biz, IAccout accout)
        {
            _ofpay = ofpay;
            _token = token;
            _log = _log = LogManager.GetLogger(Startup.repository.Name, typeof(OfpayController));
            _biz = biz;
            _accout = accout;
        }
        #endregion

        /// <summary>
        /// 获取欧飞商品
        /// author:白尚德
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("getofpaylist")]
        public async Task<H5ResponseViewModel<List<OfpayDto>>> GetOfPayList([FromBody]H5RequestViewModel obj)
        {
            H5ResponseViewModel<List<OfpayDto>> response = null;
            var code = SysCode.Ok;
            var list = await _ofpay.GetOfOayListAsync();
            response = new H5ResponseViewModel<List<OfpayDto>>(code, list.Count > 0 ? list : null);
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("mobinfo")]
        public H5ResponseViewModel<MobInfoViewModel> MobInfo([FromBody]H5RequestViewModel obj)
        {
            H5ResponseViewModel<MobInfoViewModel> response = null;
            MobInfoViewModel result = null;
            bool isLog = _token.VerifyToken((string)obj.data.userOpenId, (string)obj.data.sessionToken);
            var code = SysCode.Ok;
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{(string)obj.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                result = _ofpay.MobInfo(userLog.Phone);
            }
            else
            {
                /*短令牌失效*/
                code = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<MobInfoViewModel>(code, result ?? null);
            return response;
        }

        /// <summary>
        ///当前手机号码是否能充值当前商品
        /// author:白尚德
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("ofpayisno")]
        public async Task<H5ResponseViewModel<object>> OfPayIsNo([FromBody]H5RequestViewModel obj)
        {

            H5ResponseViewModel<object> response = null;
            var code = SysCode.Ok;
            var ofpaybyid = await _ofpay.GetOfOayByIdAsync(long.Parse((string)obj.data.ofpayId));
            long cardnums = long.Parse(ofpaybyid.Cardnum);
            var result = _ofpay.GetOfOayIsNo((string)obj.data.phone, cardnums);
            if (!result)
            {
                code = SysCode.Err;
            }
            response = new H5ResponseViewModel<object>(code, null);
            return response;
        }

        /// <summary>
        /// 话费充值
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Prepaidrefill")]
        public async Task<H5ResponseViewModel<object>> PrepaidRefill([FromBody]H5RequestViewModel obj)
        {
            H5ResponseViewModel<object> response = null;
            var code = SysCode.Ok;
            bool isLog = _token.VerifyToken((string)obj.data.userOpenId, (string)obj.data.sessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{(string)obj.data.userOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                UserLoginDto userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                //获取商品信息
                var ofpaybyid = await _ofpay.GetOfOayByIdAsync(long.Parse((string)obj.data.ofpayId));
                OfpayLogDto ofpayLog = new OfpayLogDto
                {
                    CreateTime = DateTime.Now,
                    OfPay_Id = ofpaybyid.Id,
                    Order_Id = Guid.NewGuid().ToString(),
                    Phone = userLog.Phone,
                    Status = 1,
                    UpdateTime = DateTime.Now,
                    UserId = userLog.Userid,

                };
                if (!await _ofpay.OfpayLogAsync(ofpayLog))
                {
                    //这里如果报错会直接进异常不会执行下面的代码
                    _log.Error($"话费充值接口生成本地订单错误,参数{ofpayLog}");
                }
                if (await _ofpay.PrepaidRefillAsync(ofpayLog.Phone, ofpaybyid.Cardnum, ofpayLog.Order_Id))
                {
                    ExchangeDto exchange = new ExchangeDto
                    {
                        Amount = double.Parse(ofpaybyid.Currency), //总价
                        ForderId = ofpayLog.Order_Id,
                        FuserId = userLog.Userid.ToString(),
                        Name = "虚拟商品兑出",//ofpaybyid.Name,
                        Price = double.Parse(ofpaybyid.Currency),//单价
                        Quantity = 1

                    };
                    //关闭章鱼兑换
                   // await _biz.Exchange(exchange);
                    //从php兑出
                    await _accout.Exchange_php(exchange);
                }
                else
                {
                    code = SysCode.Err;
                }
                _log.Info($"欧飞充值记录，参数为{ofpayLog}结果为{code}");
            }
            else
            {
                code = SysCode.SessionTokenLose;
            }
            response = new H5ResponseViewModel<object>(code, null);
            return response;
        }

    }
}