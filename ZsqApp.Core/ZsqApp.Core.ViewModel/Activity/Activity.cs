using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.ViewModel.Activity
{
    public class Activity
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [Display(Name = "活动名称"), Required]
        public string name { get; set; }

        /// <summary>
        /// 活动副标题
        /// </summary>
        [Display(Name = "活动副标题"), Required]
        public string title { get; set; }

        /// <summary>
        /// 页面跳转链接，可以为空
        /// </summary>
        [Display(Name = "页面跳转链接")]
        [DataType(DataType.Url)]
        public string interLinkage { get; set; }

        /// <summary>
        /// 图片跳转链接
        /// </summary>
        [Display(Name = "图片跳转链接")]
        public string img { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        [Display(Name = "活动开始时间"), Required]
        public DateTime activity_Start_Time { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [Display(Name = "活动结束时间"), Required]
        public DateTime activity_Finish_Time { get; set; }

        public int Sort { get; set; }
    }
}
