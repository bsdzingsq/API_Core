using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.Ofpay
{
    public class OfpayDto
    {

        public long Id { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 虚拟币价格
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 充值卡面额
        /// </summary>
        public string Cardnum { get; set; }

    }
}
