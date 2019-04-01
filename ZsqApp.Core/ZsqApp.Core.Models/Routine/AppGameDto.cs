using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Routine
{
    /// <summary>
    /// 游戏列表
    /// </summary>
    public class AppGameDto
    {

        public long Id { get; set; }
        /// <summary>
        /// 游戏名称
        /// </summary>
        public string GameName { get; set; }
        /// <summary>
        /// 跳转链接
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图片链接
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public int GameType { get; set; } 
        /// <summary>
        /// 竞猜事件数量
        /// </summary>
        public int MatchCount { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 跳转类型
        /// </summary>
        public int UrlType { get; set; }
    }
}
