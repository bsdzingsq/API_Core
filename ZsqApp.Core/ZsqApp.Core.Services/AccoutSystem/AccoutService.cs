using AutoMapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.AccoutSystem;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.PHPRequest;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;

namespace ZsqApp.Core.Services.AccoutSystem
{
    //**************操作记录******************
    //创建时间：
    //作者：白尚德
    //内容描述：php接口实现
    //***************************************
    public class AccoutService : IAccout
    {
        private readonly IOptions<PHPRequestSetting> _options;
        private readonly ISystems _sys;
        private readonly IMapper _mapper;
        protected readonly FunHaiNanContext _context;
        private readonly IOptions<HaiXiaSetting> _haixiaoptions;
        private readonly IOptions<HaiXiaPhpSetting> _phpoptions;

        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;
        public AccoutService(IOptions<PHPRequestSetting> options, ISystems sys, IMapper mapper, FunHaiNanContext context, IOptions<HaiXiaSetting> haixiaoptions, IOptions<HaiXiaPhpSetting> phpoptions)
        {
            _options = options;
            _sys = sys;
            _mapper = mapper;
            _context = context;
            _log = _log = LogManager.GetLogger("NETCoreRepository", typeof(AccoutService));
            _haixiaoptions = haixiaoptions;
            _phpoptions = phpoptions;
        }

        /// <summary>
        /// 账户充值
        /// author:白尚德
        /// </summary>
        /// <param name="fuserId"></param>
        /// <param name="forderId"></param>
        /// <param name="amount"></param>
        /// <param name="description"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Recharge_php(long fuserId, string forderId, double amount, string description, string key)
        {
            PHPRequest Request = new PHPRequest
            {
                AppKey = _options.Value.AppKey,
                Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(new RechargePhpRequest()
                {
                    amount = amount,
                    forderId = forderId,
                    fuserId = fuserId.ToString(),
                    description = description,
                    rateKey = key,
                }))
            };
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "recharge");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"账户充值请求参数_php{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"账户充值返回值_php{JsonHelper.SerializeObject(response)}" + Request.RequestId);
            return response.Code == "0";
        }

        /// <summary>
        /// 商品兑换
        /// author:白尚德
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public async Task<bool> Exchange_php(ExchangeDto exchange)
        {
            PHPRequest Request = new PHPRequest
            {
                AppKey = _options.Value.AppKey,
                Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(exchange))
            };
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "exchange");
            _log.Info($"商品兑换请求参数_php{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($"商品兑换返回值_php{JsonHelper.SerializeObject(response)}");
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
        /// author:白尚德
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        public async Task<bool> Refund_php(RefundDto refund)
        {
            PHPRequest Request = new PHPRequest
            {
                AppKey = _options.Value.AppKey,
                Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(refund))
            };
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "refund");
            _log.Info($"账户退款请求参数_php{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"账户退款返回值_php{JsonHelper.SerializeObject(response)}");
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
        /// 查询用户流水
        /// author：白尚德  
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="type">1000 全部、1001 收入、1002 支出、1010 中奖、1012 充值、1014 退款、1016 兑换、1017 投注</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public FundList AcquireFund_php(long userId, int type, int pageIndex, int pageSize)
        {
            PHPFundRequest Fund = new PHPFundRequest
            {
                fuserId = userId.ToString(),
                type = type
            };
            PHPRequest Request = new PHPRequest();
            Request.Page = new Models.PHPRequest.pagerequest
            {
                PageIndex = pageIndex == 0 ? 1 : pageIndex,
                PageSize = pageSize == 0 ? 20 : pageSize,
            };
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(Fund));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "accountRecord");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"查询用户流水_php请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"查询用户流水_php返回值{ JsonHelper.SerializeObject(response)}");
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
        /// 查询用户余额
        /// author：白尚德  
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>余额，可兑出</returns>
        public UserBalanceDto AcquireBalance_php(long userId)
        {
            BalanceReuqest Balance = new BalanceReuqest
            {
                FuserId = userId.ToString()
            };
            PHPRequest Request = new PHPRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(Balance));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strUrl = string.Format(_options.Value.Url, "v1", "balance");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"查询用户余额请求参数_php{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"查询用户余额返回值_php{ JsonHelper.SerializeObject(response)}");
            if (response.Code == "0")
            {
                var vResult = JsonHelper.DeserializeJsonToObject<UserBalanceDto>(_sys.Base64Decode(response.Data));
                return vResult;
            }
            return null;
        }

        /// <summary>
        /// 根据兑出订单号查询订单状态
        /// author:白尚德
        /// </summary>
        /// <param name="forderId">第三方订单号</param>
        /// <returns></returns>
        public CostStatusResult CostStatus_php(string forderId)
        {
            CostStatusResult result = null;
            CostStatusResponse costStatus = new CostStatusResponse
            {
                forderId = forderId
            };
            PHPRequest Request = new PHPRequest();
            Request.AppKey = _options.Value.AppKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(costStatus));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"根据兑出订单号查询订单状态_php请求参数{strJson}");
            string strUrl = string.Format(_options.Value.Url, "v1", "costStatus");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"根据兑出订单号查询订单状态_php返回值{JsonHelper.SerializeObject(response)}");
            result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));

            if (result.code == 0)
            {
                result.code = 0;
                result.message = "成功";
            }
            else
            {
                result = null;
            }
            return result;

        }

        /// <summary>
        /// 赠币
        /// author：白尚德 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GiveCurrencyAsync_php(GiveCurrencyDto giveCurrency)
        {
            PHPRequest Request = new PHPRequest
            {
                AppKey = _options.Value.AppKey,
                Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(giveCurrency))
            };
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_options.Value.AppSecret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_options.Value.Url, "v1", "gift");
            _log.Info($"用户赠币请求参数_php{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($"用户赠币返回值_php{JsonHelper.SerializeObject(response)}");
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
        /// 海峡竞技消费调用章鱼接口2.1
        /// </summary>
        /// <param name="consume"></param>
        /// <returns></returns>
        public async Task<bool> Consume(ConsumeDto consume)
        {
            PHPRequest Request = new PHPRequest
            {
                AppKey = _haixiaoptions.Value.appKey,
                Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(consume))
            };
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_haixiaoptions.Value.secret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_haixiaoptions.Value.url, "v1", "partnerBet");
            _log.Info($"海峡竞技消费调用章鱼请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($"海峡竞技消费调用章鱼返回值{JsonHelper.SerializeObject(response)}");
            if (result.code == 0)
            {
                UserExchangeDto temp = new UserExchangeDto
                {
                    Amount = consume.amount,
                    CreateTime = DateTime.Now,
                    Name = consume.description,
                    OrderId = consume.forderId,
                    UserId = long.Parse(consume.fuserId),
                    Price = consume.amount,
                    Quantity = 1,
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
        /// 海峡竞技消费PHP
        /// </summary>
        /// <param name="consume"></param>
        /// <returns></returns>
        public async Task<bool> Consume_php(ConsumeDto consume)
        {
            ConsumePhpDto consumephp = new ConsumePhpDto
            {
                GameKey = _phpoptions.Value.gameKey,
                GameSetId = consume.forderId,
                OrderId = consume.forderId,
                UserId = consume.fuserId,
                Amount = consume.amount,
                OperateTime = Convert.ToInt64(DateTime.Now.ToOADate()),
                Description = consume.description

            };
            PHPRequest Request = new PHPRequest();
            Request.AppKey = _phpoptions.Value.appKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(consumephp));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_phpoptions.Value.secret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            string strUrl = string.Format(_phpoptions.Value.url, "v1", "haixiaBet");
            _log.Info($" 海峡竞技消费PHP请求参数{strJson}");
            var response = JsonHelper.DeserializeJsonToObject<PHPResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            var result = JsonHelper.DeserializeJsonToObject<CostStatusResult>(_sys.Base64Decode(response.Data));
            _log.Info($" 海峡竞技消费PHP返回值{JsonHelper.SerializeObject(response)}");
            if (result.code == 0)
            {
                UserExchangeDto temp = new UserExchangeDto
                {
                    Amount = consume.amount,
                    CreateTime = DateTime.Now,
                    Name = consume.description,
                    OrderId = consume.forderId,
                    UserId = long.Parse(consume.fuserId),
                    Price = consume.amount,
                    Quantity = 1,
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
        /// 海峡竞技从章鱼获取订单状态
        /// </summary>
        /// <param name="forderId"></param>
        /// <returns></returns>
        public CostStatusResult CostOrderStatus(string forderId)
        {
            CostStatusResult result = new CostStatusResult();
            CostorderStatusResult statusResult = null;
            CostOrderStatusResponse costStatus = new CostOrderStatusResponse
            {
                forderId = forderId,
                type = "1"
            };
            ZhangYuRequest Request = new ZhangYuRequest();
            Request.AppKey = _haixiaoptions.Value.appKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(costStatus));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_haixiaoptions.Value.secret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"根据兑出订单号查询订单状态请求参数{strJson}");
            string strUrl = string.Format(_haixiaoptions.Value.url, "v1", "partnerFind");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            _log.Info($"根据兑出订单号查询订单状态返回值{JsonHelper.SerializeObject(response)}");
            statusResult = JsonHelper.DeserializeJsonToObject<CostorderStatusResult>(_sys.Base64Decode(response.Data));
            if (statusResult.status == "1")
            {
                result.code = 0;
                result.message = "成功";
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 海峡竞技从php获取订单状态
        /// </summary>
        /// <param name="forderId"></param>
        /// <returns></returns>
        public CostStatusResult CostOrderStatus_php(string forderId)
        {
            CostStatusResult result = new CostStatusResult();
            //CostorderStatusPhpResult statusResult = null;
            CostOrderStatusPhpResponse costStatus = new CostOrderStatusPhpResponse
            {
                type = (int)1,
                gameKey = _phpoptions.Value.gameKey,
                gameSetId = forderId,
                orderId = forderId
            };
            PHPRequest Request = new PHPRequest();
            Request.AppKey = _phpoptions.Value.appKey;
            Request.Data = _sys.Base64Encode(Encoding.UTF8, JsonHelper.SerializeObject(costStatus));
            Request.Sign = _sys.Sha512Encode($"{Request.AppKey}{_phpoptions.Value.secret}{Request.Data}{Request.Nonce}{Request.Timestamp}");
            string strJson = JsonHelper.SerializeObject(Request);
            _log.Info($"根据兑出订单号查询订单状态请求参数{strJson}");
            string strUrl = string.Format(_phpoptions.Value.url, "v1", "gameFind");
            var response = JsonHelper.DeserializeJsonToObject<ZhangYuResponse>(_sys.PostJsonData(strUrl, strJson, Encoding.UTF8));
            // statusResult = JsonHelper.DeserializeJsonToObject<CostorderStatusPhpResult>(response);

            _log.Info($"根据兑出订单号查询订单状态返回值{JsonHelper.SerializeObject(response)}");
            //statusResult = JsonHelper.DeserializeJsonToObject<CostorderStatusPhpResult>(_sys.Base64Decode(response.Data));
            if (response.Code == "0")
            {
                result.code = 0;
                result.message = "成功";
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 获取微信公众号充值信息
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public List<orderInfoList> GetRechargeTypes(string strJson)
        {
            try
            {
                RechargesRequest requests = new RechargesRequest();
                requests.data = strJson;
                string strJsons = JsonHelper.SerializeObject(requests);
                string strUrl = "http://192.168.1.117:9711/api/order/orderinfomation";
                _log.InfoFormat("获取微信公众号充值信息请求参数：{0}", requests);
                var response = JsonHelper.DeserializeJsonToObject<RechargesResponse>(_sys.PostJsonData(strUrl, strJsons, Encoding.UTF8));
                _log.InfoFormat("获取微信公众号充值信息返回参数：{0}", JsonHelper.SerializeObject(response));
                if (response.Code==0)
                {
                    var result = response.Data.orderInfo;
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //end
    }
}
