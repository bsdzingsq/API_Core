using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Entity.UserEntity;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace ZsqApp.Core.Services.ZhangYu
{
    //**************操作记录******************
    //创建时间：
    //作者：陶林辉
    //内容描述：章鱼接口实现
    //***************************************
    public class BizService : IBiz
    {
        private readonly IOptions<ZhangYuSetting> _options;
        private readonly ISystems _sys;
        private readonly IMapper _mapper;
        protected readonly FunHaiNanContext _context;
        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;
        public BizService(ISystems sys, IOptions<ZhangYuSetting> options, IMapper mapper, FunHaiNanContext context)
        {
            _sys = sys;
            _options = options;
            _mapper = mapper;
            _context = context;
            _log = _log = LogManager.GetLogger("NETCoreRepository", typeof(BizService));
        }

        /// <summary>
        /// 查询用户余额
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>余额，可兑出</returns>
        public UserBalanceDto AcquireBalance(long userId)
        {
            BalanceReuqest Balance = new BalanceReuqest
            {
                FuserId = userId.ToString()
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(Balance));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "balance");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"查询用户余额请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"查询用户余额返回值{ JsonHelper.SerializeObject(response)}");
            if (response.Code == "0")
            {
                var vResult = JsonHelper.DeserializeJsonToObject<UserBalanceDto>(_sys.Base64Decode(response.Data));
                return vResult;
            }
            return null;
        }


        /// <summary>
        /// 查询用户流水
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public FundList AcquireFund(long userId, int type, int pageIndex, int pageSize)
        {
            FundRequest Fund = new FundRequest
            {
                FuserId = userId,
                Type = type
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.Page = new pagerequest
            {
                PageIndex = pageIndex == 0 ? 1 : pageIndex,
                PageSize = pageSize == 0 ? 20 : pageSize,
            };
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(Fund));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "accountRecord");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"查询用户流水请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"查询用户流水返回值{ JsonHelper.SerializeObject(response)}");
            var vResult = JsonHelper.DeserializeJsonToObject<FundList>(_sys.Base64Decode(response.Data));
            if (vResult != null)
            {
                vResult.Page = new PageInfo()
                {
                    Count = response.Page.Count,
                    PageIndex = response.Page.PageIndex,
                    PageSize = response.Page.PageSize,
                    PageTotal = response.Page.PageTotal,
                    Totaltotal = response.Page.Totaltotal,
                };
            }

            return vResult;
        }

        /// <summary>
        /// 账户充值
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="orderId">订单号</param>
        /// <param name="amount">充值虚拟币金额,</param>
        /// <param name="key">充值key</param>
        /// <returns></returns>
        public bool Recharge(long userId, string orderId, double amount, string key)
        {
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(new RechargeRequest()
            {
                amount = amount / 100,
                forderId = orderId,
                fuserId = userId.ToString(),
                rateKey = key,
            }));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "recharge");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"账户充值请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"账户充值返回值{JsonHelper.SerializeObject(response)}");
            return response.Code == "0";
        }

        /// <summary>
        /// 查询用户竞猜列表
        /// author：陶林辉  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public OrderList AcquireOrder(long userId, int type, int pageIndex, int pageSize)
        {
            FundRequest Fund = new FundRequest
            {
                FuserId = userId,
                Type = type
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.Page = new pagerequest
            {
                PageIndex = pageIndex == 0 ? 1 : pageIndex,
                PageSize = pageSize == 0 ? 20 : pageSize,
            };
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(Fund));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "orderList");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"查询用户竞猜列表请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"查询用户竞猜列表返回值{JsonHelper.SerializeObject(response)}");
            if (response.Code == "0")
            {
                var vResult = JsonHelper.DeserializeJsonToObject<OrderList>(_sys.Base64Decode(response.Data));
                if (vResult != null)
                {
                    vResult.Page = new PageInfo()
                    {
                        Count = response.Page.Count,
                        PageIndex = response.Page.PageIndex,
                        PageSize = response.Page.PageSize,
                        PageTotal = response.Page.PageTotal,
                        Totaltotal = response.Page.Totaltotal,
                    };
                }
                return vResult;
            }
            return null;
        }

        /// <summary>
        /// 竞猜投注详情
        /// author：陶林辉 
        /// </summary>
        /// <param name="forderId">第三方订单 Id</param>
        /// <returns></returns>
        public Order AcquireOrderDetail(string forderId)
        {
            OrderDetailRequest OrderDetail = new OrderDetailRequest
            {
                ForderId = forderId
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(OrderDetail));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "orderDetail");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"竞猜投注详情请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"竞猜投注详情返回值{JsonHelper.SerializeObject(response)}");
            var vResult = JsonHelper.DeserializeJsonToObject<Order>(_sys.Base64Decode(response.Data));
            return vResult;
        }

        /// <summary>
        /// 获取竞猜游戏列表
        /// author：陶林辉 
        /// </summary>
        /// <returns></returns>
        public MatchTypeList AcquireGuessMatch()
        {
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = null;
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "sportTypeList");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"获取竞猜游戏列表请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"获取竞猜游戏列表返回值{JsonHelper.SerializeObject(response)}");
            if (response.Code == "0")
            {
                var vResult = JsonHelper.DeserializeJsonToObject<MatchTypeList>(_sys.Base64Decode(response.Data));
                return vResult;
            }
            return null;
        }

        /// <summary>
        /// 赠币
        /// author：林辉 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GiveCurrencyAsync(GiveCurrencyDto giveCurrency)
        {
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(giveCurrency));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "gift");
            _log.Info($"用户赠币请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($"用户赠币返回值{JsonHelper.SerializeObject(response)}");
            if (result.code == 0)
            {
                GiveCurrencyLogDto giveCurrencyLogDto = new GiveCurrencyLogDto
                {
                    CreateTime = DateTime.Now,
                    Key = giveCurrency.key,
                    Order = giveCurrency.forderId,
                    UserId = long.Parse(giveCurrency.fuserId)
                };
                var entity = _mapper.Map<GiveCurrencyLogDto, GiveCurrencyLogEntity>(giveCurrencyLogDto);
                _context.giveCurrencyLog.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 商品兑换
        /// author:林辉
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task<bool> Exchange(ExchangeDto exchange)
        {
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(exchange));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "exchange");
            _log.Info($"商品兑换请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($"商品兑换返回值{JsonHelper.SerializeObject(response)}");
            if (result.code == 0)
            {
                UserExchangeDto temp = new UserExchangeDto
                {
                    Amount = exchange.Amount,
                    CreateTime = DateTime.Now,
                    Name = exchange.Name,
                    OrderId = exchange.ForderId,
                    UserId = long.Parse(exchange.FuserId),
                    Price = exchange.Price,
                    Quantity = exchange.Quantity,
                    Status = 0 //0正常，1退款
                };
                var entity = _mapper.Map<UserExchangeDto, UserExchangeEntity>(temp);
                _context.Exchange.Add(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 账户退款
        /// author:林辉
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        public async Task<bool> Refund(RefundDto refund)
        {
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(refund));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "refund");
            _log.Info($"账户退款请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"账户退款返回值{JsonHelper.SerializeObject(response)}");
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            if (result.code == 0)
            {
                var entity = await _context.Exchange.FirstOrDefaultAsync(m => m.OrderId == refund.forderId);
                if (entity == null)
                {
                    return false;//订单不存在
                }
                entity.Status = 1;//退款
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



        /// <summary>
        /// 获取游戏地址
        /// author:林辉
        /// </summary>
        /// <param name="gameKey">游戏key</param>
        /// <param name="userId">用户id</param>
        /// <param name="version">APP版本</param>
        /// <param name="osType">来源系统</param>
        /// <param name="deviceCode">设备型号</param>
        /// <param name="nickName">用户昵称</param>
        /// <returns></returns>
        public GameUrlResponse GameUrl(string gameKey, long userId, string version, int osType, string deviceCode, string nickName)
        {
            GameUrlResult gameUrl = new GameUrlResult
            {
                deviceCode = deviceCode,
                gameKey = gameKey,
                nickName = nickName,
                osType = osType,
                version = version,
                userId = userId.ToString(),
                channelId = "010010000000000"
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(gameUrl));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"获取游戏地址请求参数{strJson}");
            string strUrl = string.Format(_options.Value.Url, "v1", "gameUrl");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"获取游戏地址返回值{JsonHelper.SerializeObject(response)}");
            var result = JsonHelper.DeserializeJsonToObject<GameUrlResponse>(_sys.Base64Decode(response.Data));
            return result;
        }

        /// <summary>
        /// 章鱼1.4用户认证
        /// author:林辉
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="userOpenId">userOpenId</param>
        /// <returns></returns>
        public string IsNoLogin(string token, string userOpenId)
        {
            IsNoLoginResult result = null;
            string url = "http://hi-api.8win.com/login/token/verify?";
            url = $"{url}userOpenId={userOpenId}&token={token}";
            result = JsonHelper.DeserializeJsonToObject<IsNoLoginResult>(_sys.PostJsonData(url, "", Encoding.UTF8));
            if (result.result == "1")
            {
                return result.u;
            }
            return "0";
        }

        /// <summary>
        /// 根据兑出订单号查询订单状态
        /// author:林辉
        /// </summary>
        /// <param name="forderId">第三方订单号</param>
        /// <returns></returns>
        public CostStatusResult CostStatus(string forderId)
        {
            CostStatusResult result = null;
            CostStatusResponse costStatus = new CostStatusResponse
            {
                forderId = forderId
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(costStatus));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"根据兑出订单号查询订单状态请求参数{strJson}");
            string strUrl = string.Format(_options.Value.Url, "v1", "costStatus");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"根据兑出订单号查询订单状态返回值{JsonHelper.SerializeObject(response)}");
            result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            return result;
        }
    }
}
