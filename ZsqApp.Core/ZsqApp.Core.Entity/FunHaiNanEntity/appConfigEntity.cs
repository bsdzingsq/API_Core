using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Entity.FunHaiNanEntity
{
    /// <summary>
    /// appkey
    /// </summary>
    public class appConfigEntity
    {
        public int Id { get; set; }

        public string Keys { get; set; }

        public string Secret { get; set; }

        public string Name { get; set; }

        public DateTime Createtime { get; set; }

        public DateTime Updatetime { get; set; }

        public string Remark { get; set; }
    }
}
