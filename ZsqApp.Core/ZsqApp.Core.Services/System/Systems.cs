using AutoMapper;
using log4net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.FunHaiNanEntity;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Channel;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Headers;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.Services.System
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：appkey接口实现
    //***************************************
    public class Systems : ISystems
    {

        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        private readonly IChannel _channel;
        private readonly ILog _log;
        public Systems(FunHaiNanContext context, IMapper mapper, IChannel channel)
        {
            _context = context;
            _mapper = mapper;
            _channel = channel;
            _log = _log = LogManager.GetLogger("NETCoreRepository", typeof(Systems));
        }

        /// <summary>
        /// 加密
        /// author：陶林辉
        /// </summary>
        /// <param name="toEncrypt">需要加密的原文</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">向量</param>
        /// <returns>加密结果</returns>
        public string AesEncrypt(string toEncrypt, string key, string iv = "8460257940005478")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// 解密
        /// author：陶林辉
        /// </summary>
        /// <param name="toDecrypt">需要解密的加密串</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">向量</param>
        /// <returns>明文</returns>
        public string AesDecrypt(string toDecrypt, string key, string iv = "8460257940005478")
        {

            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] ivArray = Encoding.UTF8.GetBytes(iv);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.BlockSize = 128;
            rDel.KeySize = 256;
            rDel.FeedbackSize = 128;
            rDel.Padding = PaddingMode.PKCS7;
            rDel.Key = keyArray;
            rDel.IV = ivArray;
            rDel.Mode = CipherMode.CBC;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// Base64加密
        /// author：陶林辉
        /// </summary>
        /// <param name="encodeType">code</param>
        /// <param name="source">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }

        /// <summary>
        /// Base64解密
        /// author：陶林辉
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public (string val, SysCode code) Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(result);
                decode = encodeType.GetString(bytes);
            }
            catch (Exception)
            {
                decode = result;
                return ("{}", SysCode.Base64DecodeErr);
            }
            return (decode, SysCode.Ok);

        }

        /// <summary>
        /// 根据AppKe获取app
        /// author：陶林辉
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public AppConfigDto GetAppConfig(string appKey)
        {
            var vEntity = _context.AppConfig.Where(c => c.Keys == appKey).FirstOrDefault();
            var vDto = _mapper.Map<appConfigEntity, AppConfigDto>(vEntity);
            return vDto;
        }

        /// <summary>
        /// MD5 32位小写
        /// author：陶林辉
        /// </summary>
        /// <param name="source">源</param>
        /// <returns></returns>
        public string Md5Encode(string source)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = Encoding.GetEncoding("utf-8").GetBytes(source);
                byte[] OutBytes = md5.ComputeHash(data);

                string OutString = "";
                for (int i = 0; i < OutBytes.Length; i++)
                {
                    OutString += OutBytes[i].ToString("x2");
                }
                return OutString.ToLower();
            }
        }

        /// <summary>
        /// 生成随机数
        /// author：陶林辉
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public int GetRandomSeed(int len = 6)
        {
            Random Rdom = new Random();
            return Rdom.Next(100001, 999999);
        }

        /// <summary>
        /// 模拟post提交Json数据
        /// author：陶林辉
        /// </summary>
        /// <param name="posturl">地址</param>
        /// <param name="postData">数据</param>
        /// <param name="encoding">encode</param>
        /// <returns></returns>
        public string PostJsonData(string posturl, string postData, Encoding encoding)
        {
            Stream outstream = null;
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            byte[] data = encoding.GetBytes(postData);

            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(posturl) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                // request.TransferEncoding = encoding.HeaderName; 
                outstream = request.GetRequestStream();
                outstream.Write(data, 0, data.Length);
                outstream.Close();
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                //string err = ex.Message;
                //LogHelper.Instance.Error("Post第三方地址" + posturl, ex.Message);
                _log.ErrorFormat("Post第三方地址:{0},错误信息:{1}", posturl, ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Sha512Encode
        /// author：陶林辉
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public string Sha512Encode(string val)
        {
            string strResult = "";
            byte[] buffer = Encoding.UTF8.GetBytes(val);//UTF-8 编码
                                                        //64字节,512位
            SHA512CryptoServiceProvider SHA512 = new SHA512CryptoServiceProvider();
            byte[] h5 = SHA512.ComputeHash(buffer);
            strResult = BitConverter.ToString(h5).Replace("-", string.Empty);
            return strResult.ToLower();

        }


        /// <summary>
        /// Base64解密
        /// author：陶林辉
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public string Base64Decode(string result)
        {
            string decode = string.Empty;
            try
            {
                byte[] bytes = Convert.FromBase64String(result);
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                decode = result;
                return "";
            }
            return decode;
        }

        /// <summary>
        /// 校验业务参数
        /// author：陶林辉
        /// </summary>
        /// <param name="data">业务参数</param>
        /// <param name="action">操作器名称</param>
        /// <returns></returns>
        public SysCode CheckParameters(dynamic data, string action)
        {
            switch (action)
            {
                case "gameUrl_v1"://中转页跳转
                    {
                        if (StringExtension.IsBlank((string)data.userOpenId) || StringExtension.IsBlank((string)data.gameType))
                        {
                            return SysCode.LackParameter;
                        }
                        int gameId = StringExtension.ToInt((string)data.gameType);
                        if (gameId <= 0 && gameId > 5)
                        {
                            return SysCode.ErrParameter;
                        }

                    }
                    break;
                case "GetValidate"://图形码
                    {
                        if (StringExtension.IsNotBlank((string)data.name))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "ContrastCode":
                    {
                        if (StringExtension.IsBlank((string)data.name) || StringExtension.IsBlank((string)data.result))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "SendCode": //获取验证码
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.type) ||
                            StringExtension.IsBlank((string)data.sendType)
                            )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (
                             !(StringExtension.ToInt((string)data.type) != 0)
                            || !(StringExtension.ToInt((string)data.type) < 7)
                            || !(StringExtension.ToInt((string)data.sendType) != 0)
                            || !(StringExtension.ToInt((string)data.sendType) < 3)
                            )
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "register"://注册
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.password) ||
                            StringExtension.IsBlank((string)data.channelId) || StringExtension.IsBlank((string)data.verifyCode)
                            )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (!StringExtension.IsPwd((string)data.password))
                        {
                            return SysCode.PwdFormatErr;
                        }
                        if (!StringExtension.IsChannelId((string)data.channelId))
                        {
                            return SysCode.ChannelErr;
                        }
                    }
                    break;

                case "RefreshToken":  //刷新token
                    {
                        if (StringExtension.IsBlank((string)data.token))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "PwdLogin": //密码登陆
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.password))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "CodeLogin": //验证码登陆
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.verifyCode))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "FundRecord": //用户流水
                    {
                        if (StringExtension.IsBlank((string)data.type) || StringExtension.IsBlank((string)data.pageIndex)
                             || StringExtension.IsBlank((string)data.pageSize)
                            )
                        {
                            return SysCode.LackParameter;
                        }

                        if (!StringExtension.Isint((string)data.type) || !StringExtension.Isint((string)data.pageSize) || !StringExtension.Isint((string)data.pageIndex))
                        {
                            return SysCode.ErrParameter;
                        }
                        if (StringExtension.ToInt((string)data.type) == 1000 || StringExtension.ToInt((string)data.type) == 1001 ||
                            StringExtension.ToInt((string)data.type) == 1002 ||
                            StringExtension.ToInt((string)data.type) == 1010 ||
                            StringExtension.ToInt((string)data.type) == 1012 ||
                            StringExtension.ToInt((string)data.type) == 1014 ||
                            StringExtension.ToInt((string)data.type) == 1016 ||
                            StringExtension.ToInt((string)data.type) == 1017
                            )
                        {

                        }
                        else
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "BindPhone": //绑定手机
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.verifyCode)
                           || StringExtension.IsBlank((string)data.verifyToken)
                          )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (!StringExtension.IsInt64((string)data.verifyCode))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "EnteringRealName": //实名认证
                    {
                        if (StringExtension.IsBlank((string)data.realName) || StringExtension.IsBlank((string)data.idCard))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsIdCard((string)data.idCard))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "Feedback"://用户反馈
                    {
                        if (StringExtension.IsBlank((string)data.opinion))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;

                case "OrderList": //获取竞猜列表
                    {
                        if (StringExtension.IsBlank((string)data.type) || StringExtension.IsBlank((string)data.pageIndex)
                           || StringExtension.IsBlank((string)data.pageSize)
                          )
                        {
                            return SysCode.LackParameter;
                        }

                        if (!StringExtension.Isint((string)data.type) || !StringExtension.Isint((string)data.pageSize) || !StringExtension.Isint((string)data.pageIndex))
                        {
                            return SysCode.ErrParameter;
                        }
                        if (StringExtension.ToInt((string)data.type) > 1 || StringExtension.ToInt((string)data.type) < 0)
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "AcquireAliPaySign"://生成支付宝订单
                    {
                        if (StringExtension.IsBlank((string)data.itemId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.itemId))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "OrderDetail": //获取单个投注详情
                    {
                        if (StringExtension.IsBlank((string)data.forderId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.forderId))
                        {
                            return SysCode.ErrParameter;
                        }

                    }
                    break;
                case "RetrievePwd": //找回密码
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.password) || StringExtension.IsBlank((string)data.verifyCode))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (!StringExtension.IsInt64((string)data.verifyCode))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "UpdatePwd": //修改密码
                    {
                        if (StringExtension.IsBlank((string)data.passwordEctype) || StringExtension.IsBlank((string)data.password) || StringExtension.IsBlank((string)data.verifyCode))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsPwd((string)data.password))
                        {
                            return SysCode.PwdFormatErr;
                        }
                    }
                    break;
                case "UpdatePwd_H5": //H5修改密码
                    {
                        if (((string)data.passwordEctype).IsBlank() || ((string)data.password).IsBlank() || ((string)data.verifyCode).IsBlank()
                            || ((string)data.token).IsBlank()
                           )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!((string)data.password).IsPwd())
                        {
                            return SysCode.PwdFormatErr;
                        }
                    }
                    break;

                case "Tokenverify": //对外用户认证
                    {
                        if (StringExtension.IsBlank((string)data.userOpenId) || StringExtension.IsBlank((string)data.sessionToken))
                        {
                            return SysCode.LackParameter;
                        }
                    };
                    break;
                case "GetUserInfo": //对外用户认证
                    {
                        if (StringExtension.IsBlank((string)data.userId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (StringExtension.ToInt((string)data.userId) == 0)
                        {
                            return SysCode.ErrParameter;
                        }
                    };
                    break;
                case "GameBet": //游戏投注
                    {
                        if (data == null || StringExtension.IsBlank((string)data.gameKey) || StringExtension.IsBlank((string)data.gameSetId) ||
                            StringExtension.IsBlank((string)data.orderId) || StringExtension.IsBlank((string)data.userId) ||
                            StringExtension.IsBlank((string)data.amount) || StringExtension.IsBlank((string)data.operateTime)
                        )
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "GamePrize": //游戏派奖
                    {
                        if (data == null || StringExtension.IsBlank((string)data.gameKey) || StringExtension.IsBlank((string)data.gameSetId) ||
                            StringExtension.IsBlank((string)data.orderId) || StringExtension.IsBlank((string)data.userId) ||
                            StringExtension.IsBlank((string)data.amount) || StringExtension.IsBlank((string)data.operateTime)
                        )
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "gameRefund": //游戏退款
                    {
                        if (data == null || StringExtension.IsBlank((string)data.gameKey) || StringExtension.IsBlank((string)data.gameSetId) ||
                            StringExtension.IsBlank((string)data.orderId) || StringExtension.IsBlank((string)data.amount)
                             || StringExtension.IsBlank((string)data.operateTime)
                        )
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "AcquireAppleProduct": //ios充值
                    {
                        if (data == null || StringExtension.IsBlank((string)data.itemId) || StringExtension.IsBlank((string)data.quantity))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.itemId) || !StringExtension.Isint((string)data.quantity))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "UploadingReceipt":  //ios上传收据
                    {
                        if (data == null || StringExtension.IsBlank((string)data.productId) || StringExtension.IsBlank((string)data.orderId)
                            || StringExtension.IsBlank((string)data.appleId) || StringExtension.IsBlank((string)data.receipt)
                            )
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "Exchange"://账户消费
                    {
                        if (data == null || StringExtension.IsBlank((string)data.name) || StringExtension.IsBlank((string)data.userOpenId)
                            || StringExtension.IsBlank((string)data.forderId) || StringExtension.IsBlank((string)data.amount)
                            || StringExtension.IsBlank((string)data.price) || StringExtension.IsBlank((string)data.quantity)
                            )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsDecimal((string)data.amount) || !StringExtension.IsDecimal((string)data.price)
                            || !StringExtension.Isint((string)data.quantity)
                            )
                        {
                            return SysCode.ErrParameter;
                        }
                    };
                    break;
                case "refund"://订单退款
                    {
                        if (data == null || StringExtension.IsBlank((string)data.forderId) || StringExtension.IsBlank((string)data.description))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "GameUrl": //获取章鱼游戏地址
                    {
                        if (data == null || StringExtension.IsBlank((string)data.gameId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (StringExtension.ToInt((string)data.gameId) < 0 || StringExtension.ToInt((string)data.gameId) > 3)
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "GetUserIdByPhone": //根据手机号码获取用户userid
                    {

                        if (data == null || StringExtension.IsBlank((string)data.phone))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "Recharge": //对外充值
                    {

                        if (data == null || StringExtension.IsBlank((string)data.amount) || StringExtension.IsBlank((string)data.orderId)
                            || StringExtension.IsBlank((string)data.userId) || StringExtension.IsBlank((string)data.payType))

                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.userId) || StringExtension.ToInt((string)data.payType) < 1 && StringExtension.ToInt((string)data.payType) > 3)
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "LoginSendCode":
                    {
                        if (data == null || !(StringExtension.ToInt((string)data.sendType) != 0) || !(StringExtension.ToInt((string)data.sendType) < 3))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "Login"://外置的登陆注册接口
                    {
                        if (data == null || StringExtension.IsBlank((string)data.verifyCode) || StringExtension.IsBlank((string)data.channelId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsChannelId((string)data.channelId))
                        {
                            return SysCode.ChannelErr;
                        }
                        if (!_channel.ChannelIsExist((string)data.channelId))
                        {
                            data.channelId = "010040000100006";
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "payrequest":
                    {
                        if (data == null || StringExtension.IsBlank((string)data.token) || StringExtension.IsBlank((string)data.id))
                        {
                            return SysCode.LackParameter;
                        }
                        if (StringExtension.ToInt((string)(data.id)) == 0)
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "userverify": //外置投注token认证
                    {

                        if (data == null || StringExtension.IsBlank((string)data.token))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "Exchange_H5": //H5支付
                    {
                        if (data == null || ((string)data.name).IsBlank() || ((string)data.token).IsBlank()
                          || ((string)data.forderId).IsBlank() || ((string)data.amount).IsBlank()
                          || ((string)data.price).IsBlank() || ((string)data.quantity).IsBlank()
                          )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!((string)data.amount).IsDecimal() || !((string)data.price).IsDecimal()
                            || !((string)data.quantity).Isint()
                            )
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "FundRecord_H5": //H5账户流水
                    {
                        if (data == null || ((string)data.token).IsBlank() || ((string)data.type).IsBlank()
                            || ((string)data.pageIndex).IsBlank() || ((string)data.pageSize).IsBlank()
                            )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!((string)data.type).Isint() || !((string)data.pageSize).Isint() || !((string)data.pageIndex).Isint())
                        {
                            return SysCode.ErrParameter;//参数格式错误
                        }
                        if (((string)data.type).ToInt() == 1000 || ((string)data.type).ToInt() == 1001 ||
                            ((string)data.type).ToInt() == 1002 ||
                            ((string)data.type).ToInt() == 1010 ||
                            ((string)data.type).ToInt() == 1012 ||
                            ((string)data.type).ToInt() == 1014 ||
                            ((string)data.type).ToInt() == 1016 ||
                            ((string)data.type).ToInt() == 1017
                            )
                        {

                        }
                        else
                        {
                            return SysCode.ErrParameter;//参数格式错误}
                        }
                    }
                    break;
                case "costStatus"://商品订单查询
                    if (data == null || ((string)data.forderId).IsBlank())
                    {
                        return SysCode.LackParameter;
                    }
                    break;
                case "gameUrl"://H5获取游戏地址
                    {

                        if (data == null || ((string)data.token).IsBlank() || ((string)data.gameId).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                        int gameId = StringExtension.ToInt((string)data.gameId);
                        if (gameId <= 0 || gameId > 3)
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "webFeedback":
                    {
                        if (data == null || ((string)data.token).IsBlank() || ((string)data.opinion).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "WeChatInfo": /*php获取微信充值商品和用户的userid*/
                    {
                        if (data == null || ((string)data.token).IsBlank() || ((string)data.rechargeId).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.rechargeId))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "PwdLogin_H5": //H5的密码登录
                    {
                        if (data == null || ((string)data.phone).IsBlank() || ((string)data.password).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "alterUserName_H5": //h5修改用户名
                    {
                        if (data == null || ((string)data.token).IsBlank() || ((string)data.name).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "ActivityList": //H5获取活动列表
                    {
                        if (data == null || ((string)data.token).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                        if (!((string)data.channel).IsBlank() && !((string)data.channel).IsChannelId())
                        {
                            return SysCode.ChannelErr;
                        }
                    }
                    break;
                case "homepage"://海南竞技首页
                    {
                        if (data == null || ((string)data.name).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "recommend"://海南竞技介绍
                    {
                        if (data == null || ((string)data.name).IsBlank())
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "userInfoTime"://海南竞技获取信息
                    {
                        if (data == null || StringExtension.IsBlank((string)data.userOpenId) || StringExtension.IsBlank((string)data.sessionToken))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "Competitive_exchange"://海南竞技消费
                    {
                        if (data == null || StringExtension.IsBlank((string)data.name) || StringExtension.IsBlank((string)data.userOpenId)
                            || StringExtension.IsBlank((string)data.sessionToken) || StringExtension.IsBlank((string)data.userId)
                           || StringExtension.IsBlank((string)data.forderId) || StringExtension.IsBlank((string)data.amount)
                           || StringExtension.IsBlank((string)data.price) || StringExtension.IsBlank((string)data.quantity)
                           )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsDecimal((string)data.amount) || !StringExtension.IsDecimal((string)data.price))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "verificationCode"://海峡竞技h5短信验证码
                    {
                        if (data == null || !(StringExtension.ToInt((string)data.sendType) != 0) || !(StringExtension.ToInt((string)data.sendType) < 3))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                    }
                    break;
                case "StraitSendCode"://海峡竞技验证码
                    {
                        if (StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.type) ||
                            StringExtension.IsBlank((string)data.sendType)
                            )
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (
                             !(StringExtension.ToInt((string)data.type) != 0)
                            || !(StringExtension.ToInt((string)data.type) < 7)
                            || !(StringExtension.ToInt((string)data.sendType) != 0)
                            || !(StringExtension.ToInt((string)data.sendType) < 3)
                            )
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "AliPaySign"://生成支付宝订单
                    {
                        if (StringExtension.IsBlank((string)data.itemId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.itemId))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "weCharPaySign"://生成海峡竞技微信订单
                    {
                        if (StringExtension.IsBlank((string)data.itemId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.itemId))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "acquireweCharPaySign"://生成乐趣海南微信订单
                    {
                        if (StringExtension.IsBlank((string)data.itemId))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.itemId))
                        {
                            return SysCode.ErrParameter;
                        }
                    }
                    break;
                case "GetRechangeType"://获取用户的充值类型
                    {
                        if (StringExtension.IsBlank((string)data.OrdersList))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "GetCoinDonation"://518与签到类别
                    {
                        if (StringExtension.IsBlank((string)data.SignList))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "GetUserIdChannel"://根据id查询渠道
                    {
                        if (StringExtension.IsBlank((string)data.userId) || StringExtension.IsBlank((string)data.stime))
                        {
                            return SysCode.LackParameter;
                        }
                        if (!StringExtension.Isint((string)data.userId))
                        {
                            return SysCode.ErrParameter;
                        }
                        if (!StringExtension.IsDate((string)data.stime))
                        {
                            return SysCode.DateTimeErr;
                        }

                    }
                    break;
                case "GetUserNumber"://根据时间获取各个渠道注册人数
                    {
                        if (StringExtension.IsBlank((string)data.StartTime) || StringExtension.IsBlank((string)data.OverTime))
                        {
                            return SysCode.LackParameter;
                        }
                    }
                    break;
                case "SubmitIbc"://提报ibc
                    {
                        if (StringExtension.IsBlank((string)data.downid) || StringExtension.IsBlank((string)data.timestamp)
                           || StringExtension.IsBlank((string)data.gameType) || StringExtension.IsBlank((string)data.phoneType))
                        {
                            return SysCode.LackParameter;
                        }
                        if (data.iBeacons==null)
                        {
                            return SysCode.EmptyParameter;
                        }
                    }
                    break;
                case "H5SubmitIbc"://h5提报ibc
                    {
                        if (StringExtension.IsBlank((string)data.downid) || StringExtension.IsBlank((string)data.timestamp)
                            || StringExtension.IsBlank((string)data.phone) || StringExtension.IsBlank((string)data.gameType)
                            || StringExtension.IsBlank((string)data.channelId))
                        {
                            return SysCode.EmptyParameter;
                        }
                        if (!StringExtension.IsMobile((string)data.phone))
                        {
                            return SysCode.PhoneFormatErr;
                        }
                        if (!StringExtension.IsChannelId((string)data.downid)|| !StringExtension.IsChannelId((string)data.downid))
                        {
                            return SysCode.ChannelErr;
                        }
                    }
                    break;
                default:
                    break;
            }
            return SysCode.Ok;
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <returns></returns>
        public string GetRandom()
        {
            object _lock = new object();
            lock (_lock)
            {
                Random ran = new Random();
                return "Tree" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ran.Next(1000, 9999).ToString();
            }
        }
    }
}
