using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
    public class ChnnelMenu
    {
        /// <summary>
        /// 渠道ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 渠道级别
        /// </summary>
        public int Pid { get; set; }
        /// <summary>
        /// 渠道级别
        /// </summary>
        public string ChennlId { get; set; }

        /// <summary>
        /// 是否是父节点
        /// </summary>
        public bool isParent { get; set; }

    }
    public class ChnnelMenuGroup
    {
        public List<ChnnelMenu> ChnnelMenus { get; set; }
    }
    public class ChnnelList
    {
        public string key { get; set; }
        public List<ChnnelMenuGroup> ChnneltreesList { get; set; }
    }
}
