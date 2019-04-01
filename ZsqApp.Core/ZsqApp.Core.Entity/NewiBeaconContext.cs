using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ZsqApp.Core.Entity.Routine;

namespace ZsqApp.Core.Entity
{
    public class NewiBeaconContext : DbContext
    {

        /// <summary>
        /// NewiBeaconContext
        /// </summary>
        public NewiBeaconContext(DbContextOptions<NewiBeaconContext> options) :
            base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<DevicesEntity> Devices { get; set; }
        public DbSet<ChannelsEntity> Channels { get; set; }
    }
}
