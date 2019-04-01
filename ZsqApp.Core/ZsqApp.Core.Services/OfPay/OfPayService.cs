using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Interfaces.OfPay;
using ZsqApp.Core.Models.Ofpay;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ZsqApp.Core.Ofpay;
using ZsqApp.Core.ViewModel.OfPay;
using ZsqApp.Core.Models;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Interfaces.System;
using Newtonsoft.Json.Linq;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Entity.Ofpay;
using ZsqApp.Core.Infrastructure.Utilities;
using System.Xml;

namespace ZsqApp.Core.Services.OfPay
{
    public class OfPayService : IOfpay
    {
        private readonly IMapper _mapper;
        protected readonly FunHaiNanContext _context;
        private readonly OfPaySetting _ofpay;
        private readonly ISystems _sys;

        public OfPayService(FunHaiNanContext context, IMapper mapper, IOptions<OfPaySetting> ofpay, ISystems sys)
        {
            _context = context;
            _mapper = mapper;
            _ofpay = ofpay.Value;
            _sys = sys;
        }

        /// <summary>
        /// 获取欧飞商品
        /// author:白尚德
        /// </summary> 
        /// <returns></returns>
        public async Task<List<OfpayDto>> GetOfOayListAsync()
        {
            var entity = await _context.Ofpay.Where(m => m.Is_no == 1).OrderByDescending(m => m.Sort).ToListAsync();
            return _mapper.Map<List<OfpayEntity>, List<OfpayDto>>(entity);
        }

        /// <summary>
        /// 获取用户手机号码及归属地
        /// author:白尚德
        /// </summary>
        /// <returns></returns>
        public MobInfoViewModel MobInfo(string mobilenum)
        {
            MobInfoViewModel result = null;
            string strResult = _sys.PostJsonData($"{_ofpay.Url}{"mobinfo.do"}{"?mobilenum="}{mobilenum}", "", Encoding.GetEncoding("gbk"));
            if (!StringExtension.IsBlank(strResult))
            {
                var str = strResult.Split("|");
                result = new MobInfoViewModel
                {
                    phone = mobilenum,
                    operators = str[1],
                    region = str[2]
                };
            }
            return result;
        }

        /// <summary>
        /// 根据商品id获取信息
        /// author:白尚德
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OfpayDto> GetOfOayByIdAsync(long id)
        {
            var result = await _context.Ofpay.FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<OfpayEntity, OfpayDto>(result);
        }

        /// <summary>
        /// 当前手机号码是否能充值当前商品
        /// author:白尚德
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        public bool GetOfOayIsNo(string phone, long price)
        {
            string str = _sys.PostJsonData($"{_ofpay.Url}telcheck.do?userid={_ofpay.Userid}&phoneno={phone}&price={price}", "", Encoding.GetEncoding("gbk"));
            if (!StringExtension.IsBlank(str))
            {
                //1#成功#0000&江苏南京      0#暂时不支持此类号码的充值#
                //var strs = str.Substring(14, 2);
                //int s = str.IndexOf('1');
                if (str.Length < 15)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 记录欧非充值订单
        /// author:白尚德
        /// </summary>
        /// <param name="ofpayLog"></param>
        /// <returns></returns>
        public async Task<bool> OfpayLogAsync(OfpayLogDto ofpayLog)
        {
            var entity = _mapper.Map<OfpayLogDto, OfpayLogEntity>(ofpayLog);
            _context.OfpayLog.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 话费充值
        /// author:陶林辉
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="cardnum">面额</param>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        public async Task<bool> PrepaidRefillAsync(string phone, string cardnum, string orderId)
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var md5 = $"{_ofpay.Userid}{_ofpay.Pwd}140101{cardnum}{orderId}{time}{phone}{_ofpay.Keystr}";
            md5 = _sys.Md5Encode(md5).ToUpper();
            var result = _sys.PostJsonData($"{_ofpay.Url}onlineorder.do?userid={_ofpay.Userid}&userpws={_ofpay.Pwd}&cardid=140101&cardnum={cardnum}&mctype=" +
                $"&sporder_id={orderId}&sporder_time={time}&game_userid={phone}&md5_str={md5}&ret_url=&version=6.0", "", Encoding.GetEncoding("gbk"));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            XmlNode root = doc.SelectSingleNode("//orderinfo");
            string state = root.SelectSingleNode("game_state").InnerText;
            if (state != "9")
            {
                var ofpayOrderId = root.SelectSingleNode("orderid").InnerText;
                decimal price = decimal.Parse(root.SelectSingleNode("ordercash").InnerText);
                await UpdateOfpayLogAsync(0, orderId, price, ofpayOrderId);
                return true;
            }
            string _ofpayOrderId = root.SelectSingleNode("orderid").InnerText;
            decimal _price = decimal.Parse(root.SelectSingleNode("ordercash").InnerText);
            await UpdateOfpayLogAsync(9, orderId, _price, _ofpayOrderId); //9是需要退款的，
            return false;

        }

        /// <summary>
        /// 更新欧非充值订单
        /// author:陶林辉
        /// </summary>
        /// <param name="status">状态</param>
        /// <param name="orderId">订单号</param>
        /// <param name="ordercash">欧飞价格</param>
        /// <param name="ofPayOrderId">欧飞订单号</param>
        /// <returns></returns>
        public async Task<bool> UpdateOfpayLogAsync(int status, string orderId, decimal ordercash, string ofPayOrderId)
        {
            var entity = _context.OfpayLog.FirstOrDefault(m => m.Order_id == orderId);
            if (entity == null) return false;
            entity.Status = status;
            entity.Ordercash = ordercash;
            entity.Ofpay_Order_Id = ofPayOrderId;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
