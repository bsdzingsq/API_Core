using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    /// <summary>
    /// 获取章鱼游戏地址请求实体
    /// </summary>
    public class GameUrlResult
    {
        public string gameKey { get; set; }
        public string userId { get; set; }
        public string nickName { get; set; }
        public string deviceCode { get; set; }
        public string osVersion { get; set; }
        public string platform { get; set; }
        public int osType { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public string channelId { get; set; }
        public string gps { get; set; }
        public string ibeacons { get; set; }

    }
}
