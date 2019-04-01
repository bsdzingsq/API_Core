using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    /// <summary>
    /// 章鱼商品订单查询请求参数
    /// </summary>
    public class CostOrderStatusResponse
    {
        public string forderId { get; set; }
        public string type { get; set; }
    }

    public class CostorderStatusResult
    {
        /// <summary>
        /// 示例：0 不存在、1 存在
        /// </summary>
        public string status { get; set; }
    }
}
