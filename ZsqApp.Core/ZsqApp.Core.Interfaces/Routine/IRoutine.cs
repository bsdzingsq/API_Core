using System.Collections.Generic;
using System.Threading.Tasks;
using ZsqApp.Core.Models.Routine;

namespace ZsqApp.Core.Interfaces.Routine
{
    //**************操作记录******************
    //创建时间：2018.02.05
    //作者：陶林辉
    //内容描述：应用相关内部接口
    //***************************************
    public interface IRoutine
    {
        /// <summary>
        /// 获取Banner列表
        /// author:陶林辉
        /// </summary>
        /// <returns></returns>
        Task<(List<BannerDto> staple, List<BannerDto> subsidiary)> GetBannerListAsync();
        /// <summary>
        /// 获取跑马灯
        /// author:刘嘉辉
        /// </summary>
        /// <returns></returns>
        Task<List<AppAnnunciateDto>> GetAppAnnunciateListAsync(string app_version);

        /// <summary>
        /// 获取功能开关
        /// author:刘嘉辉
        /// </summary>
        /// <param name="app_version"></param>
        /// <param name=""></param>
        /// <returns></returns>
        Task<List<EntranceDeployDto>> GetFunctionalSwitchListAsync(string app_version, int iplatform);

        /// <summary>
        /// 判断版本号是否可用
        /// author:陶林辉
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        bool JudgeVersionStatus(string version);

        /// <summary>
        /// 判断版本号是否在审核期
        /// author:陶林辉
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns></returns>
        Task<bool> JudgeVersionAuditStatusAsync(string version);


        /// <summary>
        /// 获取设备渠道号
        /// author:刘嘉辉
        /// amend:陶林辉
        /// </summary>
        /// <param name="major">最大值</param>
        /// <param name="minor">最小值</param>
        /// <param name="uuid">uuid</param>
        /// <returns></returns>
        Task<string> GetDevicesChannelIdAsync(int major, int minor, string uuid);

        /// <summary>
        /// 获取游戏列表
        /// author:刘嘉辉
        /// <param name="channelId">渠道号</param>
        /// <param name="channelIdType">渠道类型  0 来源设备信息  1 来源注册渠道  2 默认</param>
        /// </summary>
        /// <returns></returns>
        Task<List<AppGameDto>> GetGameListAsync(string channelId, int channelIdType);

        /// <summary>
        /// 根据用户手机号获取渠道号（先获取新渠道表如果没有获取用户注册渠道）
        /// author:刘嘉辉
        /// <param name="phone">手机号</param>
        /// </summary>
        /// <returns></returns>
        Task<string> GetUserPhoneChannelIdAsync(string phone);

        /// <summary>
        /// 版本更新
        /// </summary>
        /// <param name="versionCode">判断数字代号</param>
        /// <param name="platform"> 平台 0 ios 1Android</param>>
        /// <returns></returns>
        Task<AppVersionDto> VersionRenewalAsync(long versionCode, int platform);

        /// <summary>
        /// 海峡竞技版本更新
        /// author：白尚德
        /// </summary>
        /// <param name="versionCode">判断数字代号</param>
        /// <param name="platform"> 平台 0 ios 1Android</param>>
        /// <returns></returns>
        Task<StraitAppVersionDto> VersionUpgradesAsync(long versionCode, int platform);

    }
}
