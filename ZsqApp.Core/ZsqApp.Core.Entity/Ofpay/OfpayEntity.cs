using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZsqApp.Core.Ofpay
{
   public class OfpayEntity
    {
        [Key]
        public int Id { get; set; }
        public decimal Money { get; set; }
        public decimal Currency { get; set; }
        public string Img { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 上下架
        /// </summary>
        public int Is_no { get; set; }

        /// <summary>
        /// 充值类型
        /// </summary>
        public int Of_type { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 欧飞面值
        /// </summary>
        public decimal Cardnum { get; set; }

        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }

    }
}
