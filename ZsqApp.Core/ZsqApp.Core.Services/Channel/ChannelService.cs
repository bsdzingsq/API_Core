using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Channel;
using ZsqApp.Core.Models.Routine;

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

        /// <summary>
        /// 渠道树
        /// Author：白尚德
        /// </summary>
        /// <returns></returns>
        public async Task<List<ChnnelMenuGroup>> GetChnnelIbcAsync()
        {
            try
            {
                #region
                var channellist =await _newiBeacon.Channels.ToListAsync();//获取数据库数据
                                                              //计算一级菜单的数量
                List<ChnnelMenuGroup> chnnelMenuList = new List<ChnnelMenuGroup>();
                for (int i = 0; i < channellist.Count; i++)
                {
                    if (Convert.ToInt32(channellist[i].iBca_id.Substring(5, 10)) == 0)//一级菜单
                    {
                        ChnnelMenuGroup chnnelMenuGroup = new ChnnelMenuGroup();
                        chnnelMenuGroup.ChnnelMenus = new List<ChnnelMenu>();
                        // chnnelMenuList[i].ChnnelMenus = new List<ChnnelMenu>();
                        chnnelMenuGroup.ChnnelMenus.Add(new ChnnelMenu { Name = channellist[i].Title, ChennlId = channellist[i].iBca_id, Id = channellist[i].ID, Pid = 0, isParent = false });

                        //获取当前一级菜单下的二级菜单集合
                        var channellist2 = channellist;
                        for (int j = 0; j < channellist2.Count; j++)
                        {
                            if (channellist[i].iBca_id.Substring(0, 5) == channellist2[j].iBca_id.Substring(0, 5) && int.Parse(channellist2[j].iBca_id.Substring(5, 5)) != 0 && int.Parse(channellist2[j].iBca_id.Substring(10, 5)) == 0)
                            {
                                chnnelMenuGroup.ChnnelMenus.Add(new ChnnelMenu { Name = channellist2[j].Title, ChennlId = channellist2[j].iBca_id, Id = channellist2[j].ID, Pid = channellist[i].ID, isParent = false });
                                //获取当前二级菜单下的三级菜单集合
                                var channellist3 = channellist2;
                                for (int s = 0; s < channellist3.Count; s++)
                                {
                                    if (channellist2[j].iBca_id.Substring(0, 10) == channellist3[s].iBca_id.Substring(0, 10) && int.Parse(channellist3[s].iBca_id.Substring(10, 5)) != 0)
                                    {
                                        chnnelMenuGroup.ChnnelMenus.Add(new ChnnelMenu { Name = channellist3[s].Title, ChennlId = channellist3[s].iBca_id, Id = channellist3[s].ID, Pid = channellist2[j].ID, isParent = false });
                                    }
                                }
                            }
                        }
                        chnnelMenuList.Add(chnnelMenuGroup);
                    }
                }
                int count = 0;
                for (int i = 0; i < chnnelMenuList.Count; i++)
                {
                    for (int j = 0; j < chnnelMenuList[i].ChnnelMenus.Count; j++)
                    {
                        count++;
                    }
                }
                return chnnelMenuList;
                #endregion

            }
            catch (Exception)
            {
                return null;
            }
        }
        //end
    }
}
