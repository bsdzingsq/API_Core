using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class DevicesEntity
    {
        [Key]
        public int ID { get; set; }
        public Nullable<int> Type { get; set; }
        public string Name { get; set; }
        public string BluetoothMac { get; set; }
        public Nullable<int> ChannelsID { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> OperateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> major { get; set; }
        public Nullable<int> minor { get; set; }
        public string txPower { get; set; }
        public string Proximityuuid { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public Nullable<int> x { get; set; }
        public Nullable<int> y { get; set; }
        public Nullable<int> ressi { get; set; }
        public Nullable<int> BetteryLimit { get; set; }
        public string Is_Deploy { get; set; }
        public string Number { get; set; }
        public string Remark { get; set; }
        public Nullable<int> ScenesID { get; set; }
        public byte Battery { get; set; }
        public DateTime UpdateTimeB { get; set; }
    }
}
