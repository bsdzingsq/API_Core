using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    /// <summary>
    /// 功能卡关
    /// </summary>
    public class EntranceDeployEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public long id { get; set; }
        /// <summary>
        /// 开关类型 (是否审核期？是否显示狗狗入口？)
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 开关 0禁用 1开启
        /// </summary>
        public int enable_status { get; set; }
        /// <summary>
        /// APP版本
        /// </summary>
        public string app_version { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updatetime { get; set; }
        /// <summary>
        /// 安卓或ios  安卓0 ios1
        /// </summary>
        public int androidorios { get; set; }
    }
}
