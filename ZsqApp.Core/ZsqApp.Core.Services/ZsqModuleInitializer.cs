using AutoMapper;
using ZsqApp.Core.Entity.Activity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Entity.FunHaiNanEntity;
using ZsqApp.Core.Entity.Game;
using ZsqApp.Core.Entity.HomePage;
using ZsqApp.Core.Entity.Ofpay;
using ZsqApp.Core.Entity.Recharge;
using ZsqApp.Core.Entity.Routine;
using ZsqApp.Core.Entity.UserEntity;
using ZsqApp.Core.Models;
using ZsqApp.Core.Models.Currency;
using ZsqApp.Core.Models.Game;
using ZsqApp.Core.Models.Home;
using ZsqApp.Core.Models.Ofpay;
using ZsqApp.Core.Models.Recharge;
using ZsqApp.Core.Models.Routine;
using ZsqApp.Core.Models.User;
using ZsqApp.Core.Ofpay;
namespace ZsqApp.Core.Services
{
    public class ZsqModuleInitializer : ModuleInitializer
    {
        /// <summary>
        /// 加载AutoMapper配置
        /// </summary>
        /// <param name="config"></param>
        public override void LoadAutoMapper(IMapperConfigurationExpression config)
        {
            config.CreateMap<appConfigEntity, AppConfigDto>().ReverseMap();
            config.CreateMap<RegisterEntity, RegisterDto>().ReverseMap();
            config.CreateMap<UserLoginEntity, UserLoginDto>().ReverseMap();
            config.CreateMap<UserInfoEntity, UserInfoDto>().ReverseMap();
            config.CreateMap<UserLoginLogEntity, userLoginLogDto>().ReverseMap();
            config.CreateMap<BannerEntity, BannerDto>().ReverseMap();
            config.CreateMap<AppAnnunciateEntity, AppAnnunciateDto>().ReverseMap();
            config.CreateMap<EntranceDeployEntity, EntranceDeployDto>().ReverseMap();
            config.CreateMap<FeedbackEntity, FeedbackDto>().ReverseMap();
            config.CreateMap<Entity.Recharge.RechargeCommodityEntity, Models.Recharge.RechargeCommodityDto>().ReverseMap();
            config.CreateMap<AppVersionEntity, AppConfigDto>().ReverseMap();
            config.CreateMap<AliPayOrderEntity, AliPayOrderDto>().ReverseMap();
            config.CreateMap<RechargeEntity, RechargeDto>().ReverseMap();
            config.CreateMap<IbcGameInfoEntity, IbcGameInfoDto>().ReverseMap();
            config.CreateMap<GameInfoEntity, GameInfoDto>().ReverseMap();
            config.CreateMap<AppVersionEntity, AppVersionDto>().ReverseMap();
            config.CreateMap<GameLogEntity, GameLogDto>().ReverseMap();
            config.CreateMap<RechargeCommodityEntity, AppleProductDto>().ReverseMap();
            config.CreateMap<AppleReceiptEntity, AppleReceiptDto>().ReverseMap();
            config.CreateMap<GiveCurrencyLogEntity, GiveCurrencyLogDto>().ReverseMap();
            config.CreateMap<FiveOneRightEntity, FiveOneRightDto>().ReverseMap();
            config.CreateMap<UserSignEntity, UserSignDto>().ReverseMap();
            config.CreateMap<UserExchangeEntity, UserExchangeDto>().ReverseMap();
            config.CreateMap<OfpayEntity, OfpayDto>().ReverseMap();
            config.CreateMap<OfpayLogEntity, OfpayLogDto>().ReverseMap();
            config.CreateMap<ActivityEntity, ZsqApp.Core.ViewModel.Activity.Activity>().ReverseMap();
            config.CreateMap<StraitAppVersionEntity, StraitAppVersionDto>().ReverseMap();
            config.CreateMap<StraitHomePageEntity, HomeRiding>().ReverseMap();
            config.CreateMap<StraitHomePageEntity, HomeGames2>().ReverseMap();
            config.CreateMap<RechargeEntity, RechargeTypeDto>().ReverseMap();
            
        }
    }
}
