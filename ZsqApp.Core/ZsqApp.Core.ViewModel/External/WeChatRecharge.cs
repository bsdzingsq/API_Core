using System;
namespace ZsqApp.Core.ViewModel.External
{
    public class WeChatRecharge
    {

        /// <summary>
        /// 用户id
        /// </summary>
        /// <value>The user identifier.</value>
        public long userId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// 人民币价格
        /// </summary>
        /// <value>The money.</value>
        public decimal money { get; set; }

       /// <summary>
       /// 虚拟币数量
       /// </summary>
       /// <value>The amount.</value>
        public decimal amount { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>The phone.</value>
        public string phone { get; set; }

    }
}
