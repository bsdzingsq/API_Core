using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.SysEnum;

namespace ZsqApp.Core.Infrastructure.Utilities
{
    //**************操作记录******************
    //创建时间：2018.01.03
    //作者：陶林辉
    //内容描述：Redis帮助类
    //***************************************
    public static class RedisHelper
    {
        //private static string _strConn = "127.0.0.1";
        //private static string _strPwd = "yourpassword";
        //private static string _strConn = "120.78.156.192:6479";
        //private static string _strPwd = "distGame";
        //private static string _strConn = "114.242.17.126:7096";
        //private static string _strPwd = "zingsq123";
        //private static string _strConn = "120.79.193.71:6379";
        //private static string _strPwd = "";
        private static string _strConn = "r-wz9ae14bd2017df4.redis.rds.aliyuncs.com";
        private static string _strPwd = "L9gr0aLcPyG2E5uo";
        //private static string _strConn = "172.18.248.187:6379";
        //private static string _strPwd = "zsqtest";
        private static int _strDb = -1;
        static ConnectionMultiplexer _redis;
        static readonly object _locker = new object();

        #region 单例模式
        public static ConnectionMultiplexer Manager
        {

            get
            {
                if (_redis == null)
                {
                    lock (_locker)
                    {
                        if (_redis != null) return _redis;
                        _redis = GetManager();
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {

            if (StringExtension.IsBlank(connectionString))
            {
                connectionString = _strConn;
            }
            var options = ConfigurationOptions.Parse(connectionString);
            options.Password = _strPwd;
            return ConnectionMultiplexer.Connect(options);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 对象序列化成字符串
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        /// <summary>
        /// RedisValue序列化成对象
        /// </summary>
        /// <typeparam name="T">序列化后的对象</typeparam>
        /// <param name="value">RedisValue</param>
        /// <returns></returns>
        private static T ConvertObj<T>(RedisValue value)
        {
            if (StringExtension.IsBlank(value))
            {
                return default(T);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
        }

        /// <summary>
        /// 多个值序列化成list集合
        /// </summary>
        /// <typeparam name="T">集合对象</typeparam>
        /// <param name="values">RedisValue</param>
        /// <returns></returns>
        private static List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                if (model != null)
                    result.Add(model);
            }
            return result;
        }

        private static RedisKey[] ConvertRedisKeys(List<string> redisKeys, string prefix)
        {
            if (StringExtension.IsBlank(prefix))
            {
                return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
            }
            else
            {
                return redisKeys.Select(redisKey => (RedisKey)(prefix + ":" + redisKey)).ToArray();
            }
        }

        /// <summary>
        /// 获取要操作的库
        /// </summary>
        /// <param name="db">库，0和-1都是第一个库，1是第二个库...</param>
        /// <returns></returns>
        private static int GetOperationDB(RedisEnum db)
        {
            if (db == RedisEnum.Default)
            {
                return _strDb;
            }
            else
            {
                return (int)db;
            }
        }


        /// <summary>
        /// 获得枚举的Description
        /// </summary>
        /// <param name="value">枚举值</param>
        /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
        /// <returns>枚举的Description</returns>
        private static string GetDescription(this Enum value, Boolean nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }
        #endregion

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        public static bool KeyExists(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            try
            {
                string strFd = GetDescription(folder);
                return Manager.GetDatabase(GetOperationDB(db)).KeyExists(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 设置过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="min">过期时间，单位：分钟</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        public static bool KeyExpire(string key, int min = 600, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            try
            {
                string strFd = GetDescription(folder);
                return Manager.GetDatabase(GetOperationDB(db)).KeyExpire(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, DateTime.Now.AddMinutes(min));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 修改键
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="newKey">新键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool KeyRename(string key, string newKey, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            try
            {
                string strFd = GetDescription(folder);
                return Manager.GetDatabase(GetOperationDB(db)).KeyRename(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, StringExtension.IsBlank(strFd) ? newKey : strFd + ":" + newKey);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static IEnumerable<RedisKey> AllClear(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            return Manager.GetServer(_strConn, _strPwd).Keys(GetOperationDB(db), key);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool KeyDelete(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            try
            {
                string strFd = GetDescription(folder);
                return Manager.GetDatabase(GetOperationDB(db)).KeyDelete(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static long KeyDelete(List<string> keys, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            try
            {
                string strFd = GetDescription(folder);
                return Manager.GetDatabase(GetOperationDB(db)).KeyDelete(ConvertRedisKeys(keys, strFd));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 缓存单个字符串
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireMinutes">过期时间，单位：分钟</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool StringSet(string key, string value, int expireMinutes = 600, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).StringSet(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, value, TimeSpan.FromMinutes(expireMinutes));
        }

        /// <summary>
        /// 批量缓存字符串
        /// </summary>
        /// <param name="keysStr">键</param>
        /// <param name="valuesStr">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool StringSet(string[] keysStr, string[] valuesStr, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            var vCount = keysStr.Length;
            var vKeyValuePair = new KeyValuePair<RedisKey, RedisValue>[vCount];
            for (int i = 0; i < vCount; i++)
            {
                vKeyValuePair[i] = new KeyValuePair<RedisKey, RedisValue>(StringExtension.IsBlank(strFd) ? keysStr[i] : strFd + ":" + keysStr[i], valuesStr[i]);
            }
            return Manager.GetDatabase(GetOperationDB(db)).StringSet(vKeyValuePair);
        }

        /// <summary>
        /// 缓存限时对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="expireMinutes">过期时间，单位：分钟</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool StringSet<T>(string key, T obj, int expireMinutes = 600, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).StringSet(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, JsonHelper.SerializeObject(obj), TimeSpan.FromMinutes(expireMinutes));
        }

        /// <summary>
        /// 缓存对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="obj">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static bool StringSet<T>(string key, T obj, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).StringSet(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, JsonHelper.SerializeObject(obj));
        }

        /// <summary>
        /// 根据key获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static string StringGet(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).StringGet(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
        }

        /// <summary>
        /// 批量根据key获取
        /// </summary>
        /// <param name="keys">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static RedisValue[] StringGet(List<string> keys, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).StringGet(ConvertRedisKeys(keys, strFd));
        }

        /// <summary>
        /// 根据key获取单个对象
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static T StringGet<T>(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            string vValue = Manager.GetDatabase(GetOperationDB(db)).StringGet(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
            return ConvertObj<T>(vValue);
        }



        /// <summary>
        /// 入栈（后插入的在List前面）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        public static long ListLeftPush<T>(string key, T value, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).ListLeftPush(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, ConvertJson(value));
        }

        /// <summary>
        /// 批量入栈（后插入的在List前面）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static long ListLeftPush<T>(string key, List<T> values, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            var vRedisValues = values.Select(m => (RedisValue)ConvertJson(m)).ToArray();
            return Manager.GetDatabase(GetOperationDB(db)).ListLeftPush(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, vRedisValues);
        }

        /// <summary>
        /// 出栈（删除最前面的一个元素并返回）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static T ListLeftPop<T>(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            var vRedisValues = Manager.GetDatabase(GetOperationDB(db)).ListLeftPop(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
            return ConvertObj<T>(vRedisValues);
        }

        /// <summary>
        /// 入队（后插入的在List后面）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        public static long ListRightPush<T>(string key, T value, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).ListRightPush(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, ConvertJson(value));
        }

        /// <summary>
        /// 批量入队（后插入的在List后面）
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="values">值</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static long ListRightPush<T>(string key, List<T> values, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            var vRedisValues = values.Select(m => (RedisValue)ConvertJson(m)).ToArray();
            return Manager.GetDatabase(GetOperationDB(db)).ListRightPush(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, vRedisValues);
        }

        /// <summary>
        /// 获取
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="start">索引开始</param>
        /// <param name="stop">索引结束</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static List<T> ListRange<T>(string key, long start = 0, long stop = -1, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            var vRedisValues = Manager.GetDatabase(GetOperationDB(db)).ListRange(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key, start, stop);
            return ConvetList<T>(vRedisValues);
        }

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="folder">目录，默认根目录</param>
        /// <param name="db">库，默认读取配置文件</param>
        /// <returns></returns>
        public static long ListLength(string key, RedisFolderEnum folder = RedisFolderEnum.Root, RedisEnum db = RedisEnum.Default)
        {
            string strFd = GetDescription(folder);
            return Manager.GetDatabase(GetOperationDB(db)).ListLength(StringExtension.IsBlank(strFd) ? key : strFd + ":" + key);
        }


    }
}
