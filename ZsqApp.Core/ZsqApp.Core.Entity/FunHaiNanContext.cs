using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity.Activity;
using ZsqApp.Core.Entity.Currency;
using ZsqApp.Core.Entity.FunHaiNanEntity;
using ZsqApp.Core.Entity.Game;
using ZsqApp.Core.Entity.HomePage;
using ZsqApp.Core.Entity.Ofpay;
using ZsqApp.Core.Entity.Recharge;
using ZsqApp.Core.Entity.Routine;
using ZsqApp.Core.Entity.UserEntity;
using ZsqApp.Core.Ofpay;

namespace ZsqApp.Core.Entity
{

    public class FunHaiNanContext : DbContext
    {

        /// <summary>
        /// FunHaiNanContext
        /// </summary>
        public FunHaiNanContext(DbContextOptions<FunHaiNanContext> options) :
            base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<appConfigEntity> AppConfig { get; set; }
        public DbSet<RegisterEntity> Register { get; set; }
        public DbSet<UserLoginEntity> UserLogin { get; set; }
        public DbSet<UserInfoEntity> UserInfo { get; set; }
        public DbSet<UserLoginLogEntity> userLoginLog { get; set; }
        public DbSet<BannerEntity> Banner { get; set; }
        public DbSet<AppAnnunciateEntity> AppAnnunciate { get; set; }
        public DbSet<EntranceDeployEntity> EntranceDeploy { get; set; }
        public DbSet<FeedbackEntity> Feedback { get; set; }
        public DbSet<RechargeCommodityEntity> RechargeCommodity { get; set; }
        public DbSet<AppVersionEntity> AppVersion { get; set; }
        public DbSet<AliPayOrderEntity> AliPayOrder { get; set; }
        public DbSet<RechargeEntity> Recharge { get; set; }
        public DbSet<GameInfoEntity> GameInfo { get; set; }
        public DbSet<GameTypeInfoEntity> GameTypeInfo { get; set; }
        public DbSet<IbcGameInfoEntity> IbcGameInfo { get; set; }
        public DbSet<GameLogEntity> GameLog { get; set; }
        public DbSet<AppleReceiptEntity> AppleReceipt { get; set; }
        public DbSet<FiveOneRightEntity> FiveOneRight { get; set; }
        public DbSet<GiveCurrencyLogEntity> giveCurrencyLog { get; set; }
        public DbSet<UserSignEntity> UserSign { get; set; }
        public DbSet<UserExchangeEntity> Exchange{ get; set; }
        public DbSet<OfpayEntity> Ofpay { get; set; }
        public DbSet<OfpayLogEntity> OfpayLog { get; set; }
        public DbSet<ActivityEntity> Activity { get; set; }
        public DbSet<Activity_ChannelEntity> Activity_Channel { get; set; }
        public DbSet<StraitAppVersionEntity> StraitAppVersion { get; set; }
        public DbSet<StraitHomePageEntity> StraitHomePage { get; set; }

    }
}
