using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Routine;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models.Routine;
namespace ZsqApp.Core.Services.Routine
{
    //**************操作记录******************
    //创建时间：2018.02.05
    //作者：陶林辉
    //内容描述：应用相关内部接口
    //***************************************
    public class RoutineService : IRoutine
    {
        protected readonly FunHaiNanContext _context;
        private readonly IMapper _mapper;
        protected readonly NewiBeaconContext _ibccontext;
        protected readonly HainanContext _hncontext;
        private readonly IBiz _biz;
        public RoutineService(FunHaiNanContext context, IMapper mapper, NewiBeaconContext ibccontext, HainanContext hainanContext, IBiz biz)
        {
            _context = context;
            _mapper = mapper;
            _ibccontext = ibccontext;
            _hncontext = hainanContext;
            _biz = biz;
        }

        /// <summary>
        /// 获取Banner列表
        /// author:陶林辉
        /// </summary>
        /// <returns></returns>
        public async Task<(List<BannerDto> staple, List<BannerDto> subsidiary)> GetBannerListAsync()
        {
            var vStapleLsit = await _context.Banner.Where(m => m.Banner_type == 0 && m.Is_status == 1).OrderByDescending(t => t.Rank).ToListAsync();
            var vSubsidiaryList = await _context.Banner.Where(m => m.Banner_type == 1 && m.Is_status == 1).OrderByDescending(t => t.Rank).ToListAsync();
            return (_mapper.Map<List<BannerEntity>, List<BannerDto>>(vStapleLsit), _mapper.Map<List<BannerEntity>, List<BannerDto>>(vSubsidiaryList));
        }
        /// <summary>
        /// 获取跑马灯
        /// author:刘嘉辉
        /// </summary>
        /// <returns></returns>
        public async Task<List<AppAnnunciateDto>> GetAppAnnunciateListAsync(string app_version)
        {
            var vNoticeList = await _context.AppAnnunciate.Where(c => c.app_version == app_version).ToListAsync();
            return _mapper.Map<List<AppAnnunciateEntity>, List<AppAnnunciateDto>>(vNoticeList);
        }
        /// <summary>
        /// 获取功能开关
        /// author:刘嘉辉
        /// </summary>
        /// <param name="app_version">版本</param>
        /// <param name="iplatform">平台</param>
        /// <returns></returns>
        public async Task<List<EntranceDeployDto>> GetFunctionalSwitchListAsync(string app_version, int iplatform)
        {
            var vSwitchList = await _context.EntranceDeploy.Where(c => c.app_version == app_version && c.androidorios == iplatform).ToListAsync();
            return _mapper.Map<List<EntranceDeployEntity>, List<EntranceDeployDto>>(vSwitchList);
        }

        /// <summary>
        /// 判断版本号是否可用
        /// author:陶林辉
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        public bool JudgeVersionStatus(string version)
        {
            ///Present_status(当前状态，0禁用，1可用，2审核)
            var entity = _context.AppVersion.Where(m => m.Version == version).FirstOrDefault();
            if (entity != null)
            {
                return entity.Present_status != 0;
            }
            return false;
        }

        /// <summary>
        /// 判断版本号是否在审核期
        /// author:陶林辉
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        /// 
        public async Task<bool> JudgeVersionAuditStatusAsync(string version)
        {
            ///Present_status(当前状态，0禁用，1可用，2审核)
            var entity = await _context.AppVersion.Where(m => m.Version == version).FirstOrDefaultAsync();
            if (entity != null)
            {
                return entity.Present_status == 2;
            }
            return false;
        }

        /// <summary>
        /// 获取设备渠道号，必须是部署的
        /// author:刘嘉辉
        /// amend:陶林辉
        /// </summary>
        /// <param name="major">最大值</param>
        /// <param name="minor">最小值</param>
        /// <param name="uuid">uuid</param>
        /// <returns></returns>
        public async Task<string> GetDevicesChannelIdAsync(int major, int minor, string uuid)
        {
            var devices = await _ibccontext.Devices.Where(m => m.major == major && m.minor == minor && m.Proximityuuid == uuid && m.ChannelsID != 1).FirstOrDefaultAsync();
            if (devices != null)
            {
                var ChannelId = await _ibccontext.Channels.Where(c => c.ID == devices.ChannelsID.Value).FirstOrDefaultAsync();
                return ChannelId.iBca_id;
            }
            return null;
        }

        /// <summary>
        /// 获取游戏列表
        /// author:刘嘉辉
        /// <param name="channelId">渠道号</param>
        /// <param name="channelIdType">渠道类型  0 来源设备信息  1 来源注册渠道  2 默认</param>
        /// </summary>
        /// <returns></returns>
        public async Task<List<AppGameDto>> GetGameListAsync(string channelId, int channelIdType)
        {
            List<AppGameDto> GameList = new List<AppGameDto>();
            List<AppGameDto> GameListTemp = new List<AppGameDto>();
            int iLevel = StringExtension.IsChannelIdLevel(channelId) + 1;
            do
            {
                iLevel--;
                string strNewChannelId = StringExtension.UpdateChannelId(channelId, iLevel);
                /*查询渠道关联游戏*/
                var ibcGameInfo = await _context.IbcGameInfo.Where(c => c.channelid == strNewChannelId && c.Status == 1).FirstOrDefaultAsync();
                if (ibcGameInfo != null)
                {
                    /*游戏数组*/
                    string[] gameKeyArray;
                    var GuessMatch = _biz.AcquireGuessMatch(); //从章鱼拿到竞猜数量数                           
                    switch (channelIdType)
                    {
                        case 0:
                            gameKeyArray = ibcGameInfo.Ibc_gameid.Split(',');
                            break;
                        case 1:
                            gameKeyArray = ibcGameInfo.Register_gameid.Split(',');
                            break;
                        default:
                            gameKeyArray = ibcGameInfo.Default_gameid.Split(',');
                            break;
                    }

                    long[] arrId = Array.ConvertAll<string, long>(gameKeyArray, m => long.Parse(m));
                    var gameInfoEntity = await _context.GameInfo.Where(c => arrId.Contains(c.Id) && c.Stateus == 1).ToListAsync();
                    foreach (var item in gameInfoEntity)
                    {
                        var guessMatchInfo = GuessMatch != null ? GuessMatch.data.Where(c => c.displayName == item.Game_Name).FirstOrDefault() : null; //获取游戏数量
                        int guessMatchCount = guessMatchInfo == null ? 1 : guessMatchInfo.matchCount;
                        if (guessMatchCount != 0 || guessMatchInfo == null)
                        {
                            AppGameDto appGameDto = new AppGameDto
                            {
                                GameName = item.Game_Name, //游戏名称
                                GameType = item.Game_type_id, //游戏类型id
                                ImgUrl = item.Icon_url, //游戏图片链接
                                Url = item.Target_url,//游戏跳转链接
                                MatchCount = guessMatchInfo == null ? 0 : guessMatchInfo.matchCount, //竞猜游戏数量
                                description = item.Description, //游戏描述
                                UrlType = item.Url_type, //跳转方式
                                Id = item.Id
                            };
                            //&& appGameDto.GameName != "狗狗冲冲冲"
                            //    && && appGameDto.GameName != "海南四星"
                            if (appGameDto.MatchCount == 0)
                            {
                                if (appGameDto.GameType == 4)
                                {
                                    GameListTemp.Add(appGameDto);
                                    continue;
                                }
                                continue;
                            }
                            GameListTemp.Add(appGameDto);

                        }
                    }
                    //排序游戏。
                    for (int i = 0; i < arrId.Length; i++)
                    {
                        for (int j = 0; j < GameListTemp.Count; j++)
                        {
                            if (arrId[i] == GameListTemp[j].Id)
                            {
                                GameList.Add(GameListTemp[j]);
                            }
                        }
                    }
                    return GameList;

                }
            } while (iLevel > 1);

            return null;
        }

        /// <summary>
        /// 根据用户手机号获取渠道号（先获取新渠道表如果没有获取用户注册渠道）
        /// author:刘嘉辉
        /// <param name="phone">手机号</param>
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetUserPhoneChannelIdAsync(string phone)
        {
            string ChannelId = "";
            long userId = await _hncontext.UserSynch.Where(c => c.phone == phone).Select(c => c.userId).FirstOrDefaultAsync();
            if (userId != 0)
            {
                ChannelId = await _hncontext.NewUserChannel.Where(c => c.UserId == userId).Select(c => c.NewChannelId).FirstOrDefaultAsync();
                if (ChannelId == null)
                {
                    ChannelId = await _hncontext.UserSynch.Where(c => c.phone == phone).Select(c => c.channelId).FirstOrDefaultAsync();
                }
            }
            return ChannelId;
        }

        /// <summary>
        /// 版本更新
        /// </summary>
        /// <param name="versionCode">判断数字代号</param>
        /// <param name="platform"> 平台 0 ios 1Android</param>
        /// <returns></returns>
        public async Task<AppVersionDto> VersionRenewalAsync(long versionCode, int platform)
        {
            try
            {
                var entity = await _context.AppVersion.Where(m => m.Platform == platform).OrderByDescending(c => c.Version_code).FirstOrDefaultAsync();
                if (versionCode < entity.Version_code)
                {
                    return _mapper.Map<AppVersionEntity, AppVersionDto>(entity);
                }
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 版本更新
        /// </summary>
        /// <param name="versionCode">判断数字代号</param>
        /// <param name="platform"> 平台 0 ios 1Android</param>
        /// <returns></returns>
        public async Task<StraitAppVersionDto> VersionUpgradesAsync(long versionCode, int platform)
        {
            var entity = await _context.StraitAppVersion.Where(m => m.Platform == platform).OrderByDescending(c => c.Version_code).FirstOrDefaultAsync();
            if (versionCode < entity.Version_code)
            {
                return _mapper.Map<StraitAppVersionEntity, StraitAppVersionDto>(entity);
            }
            return null;
        }
    }
}
