using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Entity.Routine
{
    /// <summary>
    /// 跑马灯
    /// </summary>
    public class AppAnnunciateEntity
    {
        [Key]
        public long id { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get;set; }
        /// <summary>
        /// 版本号
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
    }
}
