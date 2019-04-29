using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZsqApp.Core.Entity.MongoEntity;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Infrastructure.SysEnum;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Interfaces.Routine;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.Interfaces.ZhangYu;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Routine;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Models.ZhangYuRequest;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.ViewModel.Routine;
using ZsqApp.Core.WebApi.Filters;
using ZsqApp.Core.WebApi.Model;
using ZsqApp.Core.WebApi.Utilities;
using static ZsqApp.Core.Entity.MongoEntity.ibeaconlocus;

namespace ZsqApp.Core.WebApi.Controllers
{
    /// <summary>
    /// APP相关接口
    /// </summary>
    [Produces("application/json")]
    [Route("api/routine")]
    [ActionApiFilterAttribute]
    [EnableCors("any")]
    public class RoutineController : BaseController
    {
        #region dependency injection
        /// <summary>
        /// 初始化IRoutine
        /// </summary>
        protected readonly IRoutine _routine;
        /// <summary>
        /// 初始化ISystems
        /// </summary>
        protected readonly ISystems _sys;
        /// <summary>
        /// 初始化IToken
        /// </summary>
        private readonly IToken _token;
        /// <summary>
        /// IUser
        /// </summary>
        protected readonly IUser _user;
        /// <summary>
        /// 章鱼
        /// </summary>
        protected readonly IBiz _biz;

        /// <summary>
        /// lo4
        /// </summary>
        private readonly ILog _log;

        private readonly GameKeySetting _gameKey;
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="sys"></param>
        /// <param name="token"></param>
        /// <param name="user"></param>
        /// <param name="biz"></param>
        /// <param name="gameKey"></param>
        public RoutineController(IRoutine routine, ISystems sys, IToken token, IUser user, IBiz biz, IOptions<GameKeySetting> gameKey)
        {
            _routine = routine;
            _sys = sys;
            _token = token;
            _user = user;
            _biz = biz;
            _gameKey = gameKey.Value;
            _log = _log = LogManager.GetLogger(Startup.repository.Name, typeof(RoutineController));
        }
        #endregion

        /// <summary>
        /// 获取Banner列表
        /// author：陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireBanner")]
        public async Task<ResponseViewModel<BannerListView>> AcquireBanner([FromBody]RequestViewModel obj)
        {
            BannerListView result = new BannerListView();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<BannerListView> response = null;
            (result.Staple, result.Subsidiary) = await _routine.GetBannerListAsync();
            response = new ResponseViewModel<BannerListView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取跑马灯
        /// author：刘嘉辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("acquireAppAnnunciate")]
        public async Task<ResponseViewModel<AppAnnunciateListView>> AcquireAppAnnunciate([FromBody]RequestViewModel obj)
        {
            AppAnnunciateListView result = new AppAnnunciateListView();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<AppAnnunciateListView> response = null;
            result.Notice = await _routine.GetAppAnnunciateListAsync(obj.Client.Version);
            response = new ResponseViewModel<AppAnnunciateListView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 功能开关
        /// author：刘嘉辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("functionalSwitch")]
        public async Task<ResponseViewModel<EntranceDeployListView>> FunctionalSwitch([FromBody]RequestViewModel obj)
        {
            EntranceDeployListView result = new EntranceDeployListView();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<EntranceDeployListView> response = null;
            int iplatform = obj.Client.Platform == "Android" ? 0 : 1;
            result.SwitchList = await _routine.GetFunctionalSwitchListAsync(obj.Client.Version, iplatform);
            response = new ResponseViewModel<EntranceDeployListView>(SysCode.Ok, result, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取游戏列表
        /// author：刘嘉辉
        /// amend:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameswitch")]
        public async Task<ResponseViewModel<AppGameListView>> GameSwitch([FromBody]RequestViewModel obj)
        {
            AppGameListView result = new AppGameListView();
            result.GameList = new List<AppGameDto>();
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<AppGameListView> response = null;
            var code = SysCode.Ok;
            int channelIdType = 3; //0 来源设备信息  1 来源注册渠道  
            string strChannels = string.Empty;
            dynamic ibcList = obj.Data.iBeaconsList; //蓝牙设备集合
            if (!obj.HendInfo.UserOpenId.IsBlank())
            {
                /*登陆后查询设计信息*/
                foreach (var item in ibcList)
                {
                    channelIdType = 0;
                    //查找已经部署的ibc的渠道
                    strChannels = await _routine.GetDevicesChannelIdAsync(int.Parse((string)item.major), int.Parse((string)item.minor), (string)item.uuid);
                    if (!strChannels.IsBlank())
                    {
                        result.GameList = await _routine.GetGameListAsync(strChannels, channelIdType);
                        break;
                    }

                }
                if (result.GameList == null || result.GameList.Count == 0)
                {
                    /*设备信息为空找注册渠道*/
                    channelIdType = 1;
                    string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                    if (!strJson.IsBlank())
                    {
                        UserLoginDto UserLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                        RegisterDto Register = await _user.GetRegisterAsync(UserLog.Userid);
                        result.GameList = await _routine.GetGameListAsync(Register.Channel, channelIdType);
                    }
                }
            }
            if (result.GameList == null || result.GameList.Count == 0)
            {

                /*没有登陆直接返回原始渠道游戏列表*/
                channelIdType = 2;
                result.GameList = await _routine.GetGameListAsync("000000000000000", channelIdType);
            }

            response = new ResponseViewModel<AppGameListView>(code, result, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 版本更新
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("versiondtorenewal")]
        public async Task<ResponseViewModel<AppVersionDto>> VersionDtorenewal([FromBody]RequestViewModel obj)
        {
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<AppVersionDto> response = null;
            var code = SysCode.Ok;
            AppVersionDto appVersion = new AppVersionDto();
            int platform = obj.Client.Platform == "Android" ? 1 : 0;
            long versionCode = long.Parse(obj.Client.VersionCode);
            appVersion = await _routine.VersionRenewalAsync(versionCode, platform);
            if (appVersion != null)
            {
                appVersion.Download_url = $"{appVersion.Download_url}{obj.Client.Channel}-release.apk";
            }
            response = new ResponseViewModel<AppVersionDto>(code, appVersion ?? null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 获取游戏地址
        /// author:陶林辉
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("gameUrl")]
        public async Task<ResponseViewModel<GameUrlResponse>> GameUrl([FromBody]RequestViewModel obj)
        {

            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<GameUrlResponse> response = null;
            GameUrlResponse result = new GameUrlResponse();
            var code = SysCode.Ok;
            bool isLog = _token.VerifyToken(obj.HendInfo.UserOpenId, obj.HendInfo.SessionToken);
            if (isLog)
            {
                string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                var userInfo = await _user.SearchUserInfoAsync(userLog.Userid);
                string temp = obj.Data.gameId;
                string key = string.Empty;
                switch (temp)
                {
                    case "1":
                        key = _gameKey.Dog;
                        result.Url = "http://hainan.funhainan.com:8084/ZsqImage/loading/index.html?gameType=3";
                        // result.Url = "https://hainan.funhainan.com/ZsqImage/newapploading/index.html?gameType=3";
                        break;
                    case "2":
                        key = _gameKey.Fish;
                        result.Url = "http://hainan.funhainan.com:8084/ZsqImage/loading/index.html?gameType=2";
                        // result.Url = "https://hainan.funhainan.com/ZsqImage/newapploading/index.html?gameType=2";
                        break;
                    case "3":
                        key = _gameKey.Star;
                        break;
                    default:
                        break;
                }
                if (key == _gameKey.Star)
                {
                    int osType = obj.Client.OsType == "IOS" ? 1 : obj.Client.OsType == "Android" ? 2 : 0;
                    result = _biz.GameUrl(key, userLog.Userid, obj.Client.Version, osType, obj.Client.DeviceCode, userInfo.Nick_name);
                }
                if (result == null)
                {
                    code = SysCode.Err;
                }
            }
            else
            {
                code = SysCode.SessionTokenLose;
            }
            response = new ResponseViewModel<GameUrlResponse>(code, result ?? null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 海峡竞技版本更新
        /// author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("versionUpgrades")]
        public async Task<ResponseViewModel<StraitAppVersionDto>> VersionUpgrades([FromBody]RequestViewModel obj)
        {
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            ResponseViewModel<StraitAppVersionDto> response = null;
            var code = SysCode.Ok;
            StraitAppVersionDto appVersion = new StraitAppVersionDto();
            int platform = obj.Client.Platform == "Android" ? 1 : 0;
            long versionCode = long.Parse(obj.Client.VersionCode);
            appVersion = await _routine.VersionUpgradesAsync(versionCode, platform);
            if (appVersion != null)
            {
                appVersion.Download_url = $"{appVersion.Download_url}{obj.Client.Channel}-release.apk";
            }
            response = new ResponseViewModel<StraitAppVersionDto>(code, appVersion ?? null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        /// <summary>
        /// 提报ibc
        /// author:白尚德
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SubmitIbc")]
        public async Task<ResponseViewModel<object>> SubmitIbc([FromBody]RequestViewModel obj)
        {
            ResponseViewModel<object> response = null;
            obj = JsonHelper.DeserializeJsonToObject<RequestViewModel>(Content(User.Identity.Name).Content);
            var sysCode = SysCode.Ok;
            ibeaconlocus ibeaconlocus = new ibeaconlocus();
            var channelId = obj.Client.Channel;
            var userOpenId = obj.HendInfo.UserOpenId;
            var sessionToken = obj.HendInfo.SessionToken;
            var timesTamp = obj.Data.timestamp;
            string strJson = RedisHelper.StringGet($"{CacheKey.Token}{obj.HendInfo.UserOpenId}", RedisFolderEnum.token, RedisEnum.Three);
            if (strJson != null)
            {
                var userLog = JsonHelper.DeserializeJsonToObject<UserLoginDto>(strJson);
                dynamic ibcList = obj.Data.iBeacons;

                string strjson = JsonHelper.SerializeObject(ibcList);
                string strChannels = string.Empty;
                bool result = false;

                foreach (var item in ibcList)
                {
                    /*查找已经部署的ibc的渠道*/
                    strChannels = await _routine.GetDevicesChannelIdAsync(int.Parse((string)item.major), int.Parse((string)item.minor), (string)item.uuid);
                    /*更新设备电量 */
                    // result = await _routine.UpdateIbcAsync(int.Parse((string)item.major), int.Parse((string)item.minor), (string)item.uuid, int.Parse((string)item.battery));
                }
                _log.InfoFormat("查询设备渠道结果:{0}更新设备电量结果:{1}", strChannels, result);
                try
                {
                    var resignChannel = await _routine.GetUserIdChannelIdAsync(userLog.Userid);

                    var downId = obj.Data.downid;
                    var gps = obj.Client.Gps;
                    var userPhone = userLog.Phone;
                    var PhoneType = obj.Data.phoneType;
                    var gameType = obj.Data.gameType;
                    ibeaconlocus.downid = downId;
                    ibeaconlocus.userId = userLog.Userid;
                    ibeaconlocus.iBeacons = JsonHelper.DeserializeJsonToObject<List<ibcinfoList>>(strjson);
                    ibeaconlocus.phone = userPhone;
                    ibeaconlocus.gps = gps;
                    ibeaconlocus.timestamp = timesTamp;
                    ibeaconlocus.token = sessionToken;
                    ibeaconlocus.userOpenid = userOpenId;
                    ibeaconlocus.phoneType = PhoneType;
                    ibeaconlocus.gameType = gameType;
                    ibeaconlocus.ibcChannel = strChannels;
                    ibeaconlocus.channelId = resignChannel;
                    ibeaconlocus.createTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                    /* 将ibc信息写入mongo数据库*/
                    _routine.WriteIbcInfo(ibeaconlocus);
                }
                catch (System.Exception ex)
                {
                    _log.ErrorFormat("提报ibc接口异常{0}", ex.Message);
                    sysCode = SysCode.Err;
                }    
            }
            else
            {
                sysCode = SysCode.UserOpenIdisNo;
            }

            response = new ResponseViewModel<object>(sysCode, null, obj.Encrypt, _sys, obj.Secret);
            return response;
        }

        //end
    }
}