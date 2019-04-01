using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
   public class IBeaconsDto
    {
        public string major { get; set; }

        public string minor { get; set; }
        public string name { get; set; }
        public string rssi { get; set; }
        public string txPower { get; set; }
        public string uuid { get; set; }
    }

    public class IBeaconsList
    {
        public List<IBeaconsDto> iBeaconsList { get; set; }
    }
}
