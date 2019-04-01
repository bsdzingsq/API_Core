using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    /// <summary>
    /// 章鱼商品订单查询请求参数
    /// </summary>
    public class CostStatusResponse
    {
        public string forderId { get; set; }
    }

    public class CostStatusResult
    {
        /// <summary>
        /// 0 成功、1 处理中、2 失败
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 示例：0 成功、1 处理中、2 失败详细描述
        /// </summary>
        public string message { get; set; }
    }
}
