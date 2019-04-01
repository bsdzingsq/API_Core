using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZsqApp.Core.Interfaces.Channel
{
    /// <summary>
    /// 渠道相关接口
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// 判断渠道是否在数据库存在
        /// author：林辉
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
       bool ChannelIsExist(string channel);
    }
}
