using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    public class ChannelsEntity
    {
        public int ID { get; set; }
        public Nullable<int> Type { get; set; }
        public string Title { get; set; }
        public Nullable<int> RolesID { get; set; }
        public string Number { get; set; }
        public Nullable<int> Level { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> Status { get; set; }
        public string iBca_id { get; set; }
        public string Login { get; set; }
        public string PassWord { get; set; }
    }
}
