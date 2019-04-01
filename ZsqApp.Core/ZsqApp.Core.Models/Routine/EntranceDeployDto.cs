using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
    public class EntranceDeployDto
    {
        /// <summary>
        /// 开关类型
        /// </summary>
        public int category { get; set; }
        /// <summary>
        /// 是否开启
        /// </summary>
        public int enable_status { get; set; }
    }
}
