using System;
using System.ComponentModel.DataAnnotations;

namespace ZsqApp.Core.Entity.Activity
{
    public class ActivityEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 活动副标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 页面跳转链接，可以为空
        /// </summary>
        public string InterLinkage { get; set; }

        /// <summary>
        /// 图片跳转链接
        /// </summary>
        public string Img { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime Activity_Start_Time { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime Activity_Finish_Time { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 活动排序，99最靠前
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否显示。0显示。1显示
        /// </summary>
        public int Display { get; set; }

        /// <summary>
        /// 活动类型外键
        /// </summary>
        public int Activity_Type_Id { get; set; }

        /// <summary>
        /// 参与类型外键
        /// </summary>
        public int Participation_Type_Id { get; set; }

        /// <summary>
        /// 活动创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 活动最后修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
