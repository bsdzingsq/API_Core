using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Models;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.Interfaces.System
{
    //**************操作记录******************
    //创建时间：2018.01.02
    //作者：陶林辉
    //内容描述：appkey
    //***************************************
    public interface ISystems
    {
        /// <summary>
        /// 判断key是否存在
        /// author：陶林辉
        /// </summary>
        /// <param name="appKey">appkey</param>
        /// <returns></returns>
        AppConfigDto GetAppConfig(string appKey);

        /// <summary>
        /// 加密
        /// author：陶林辉
        /// </summary>
        /// <param name="toEncrypt">需要加密的原文</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">向量</param>
        /// <returns>加密结果</returns>
        string AesEncrypt(string toEncrypt, string key, string iv = "8460257940005478");

        /// <summary>
        /// 解密
        /// author：陶林辉
        /// </summary>
        /// <param name="toDecrypt">需要解密的加密串</param>
        /// <param name="key">加密key</param>
        /// <param name="iv">向量</param>
        /// <returns>明文</returns>
        string AesDecrypt(string toDecrypt, string key, string iv = "8460257940005478");

        /// <summary>
        /// Base64解密，采用utf8编码方式解密
        /// author：陶林辉
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        string Base64Encode(Encoding encodeType, string source);

        /// <summary>
        /// Base64解密
        /// author：陶林辉
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        (string val, SysCode code) Base64Decode(Encoding encodeType, string result);

        /// <summary>
        /// Base64解密
        /// author：陶林辉
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        string Base64Decode( string result);

        /// <summary>
        /// 校验业务参数
        /// author：陶林辉
        /// </summary>
        /// <param name="data">业务参数</param>
        /// <param name="action">操作器名称</param>
        /// <returns></returns>
        SysCode CheckParameters(dynamic data, string action);

        /// <summary>
        /// MD5
        /// author：陶林辉
        /// </summary>
        /// <param name="source">数据</param>
        /// <returns></returns>
        string Md5Encode(string source);

        /// <summary>
        /// 生成随机数
        /// author：陶林辉
        /// </summary>
        /// <param name="len">长度</param>
        /// <returns></returns>
        int GetRandomSeed(int len = 6);

        /// <summary>
        /// 模拟post提交Json数据
        /// author：陶林辉
        /// </summary>
        /// <param name="posturl">地址</param>
        /// <param name="postData">数据</param>
        /// <param name="encoding">encode</param>
        /// <returns></returns>
        string PostJsonData(string posturl, string postData, Encoding encoding);

        /// <summary>
        /// Sha512Encode
        /// author：陶林辉
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        string Sha512Encode(string val);

    }
}
