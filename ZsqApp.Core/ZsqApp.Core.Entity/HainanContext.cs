using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity.UserEntity;

namespace ZsqApp.Core.Entity
{
    public class HainanContext : DbContext
    {
        public HainanContext(DbContextOptions<HainanContext> options) :
            base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<UserTempEntity> UserTemp { get; set; }
        public virtual DbSet<NewUserChannelEntity> NewUserChannel { get; set; }
        public virtual DbSet<UserSynchEntity> UserSynch { get; set; }
        public virtual DbSet<GiveCurrencyEntity> GiveCurrency { get; set; }
        public virtual DbSet<UserSignsEntity> UserSign { get; set; }
    }
}
