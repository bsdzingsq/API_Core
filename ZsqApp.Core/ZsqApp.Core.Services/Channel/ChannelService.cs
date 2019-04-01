using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Interfaces.Channel;

namespace ZsqApp.Core.Services.Channel
{
    /// <summary>
    /// 渠道相关接口实现
    /// </summary>
    public class ChannelService : IChannel
    {
        /// <summary>
        /// 渠道库
        /// </summary>
        private readonly NewiBeaconContext _newiBeacon;

        public ChannelService(NewiBeaconContext newiBeacon)
        {
            _newiBeacon = newiBeacon;
        }

        /// <summary>
        /// 判断渠道是否存在
        /// author:林辉
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public bool ChannelIsExist(string channel)
        {
            return _newiBeacon.Channels.Any(m => m.iBca_id == channel);
        }
    }
}
