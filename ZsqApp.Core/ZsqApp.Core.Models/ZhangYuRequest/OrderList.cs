using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.ZhangYuRequest
{
    public class OrderList
    {
        public List<Order> Data { get; set; }

        public PageInfo Page { get; set; }

    }

    public class Order
    {
        /// <summary>
        /// 章鱼订单id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 订单 Id
        /// </summary>
        public string ForderId { get; set; }

        /// <summary>
        /// 竞猜名称  如：葡萄牙 vs 威尔士
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 投注选项  如：葡萄牙(-1) 10.05
        /// </summary>
        public string DisplayBet { get; set; }

        /// <summary>
        /// 投注游戏币
        /// </summary>
        public int Bets { get; set; }

        /// <summary>
        /// 盘口类型  -1 赛前盘、0 走地盘
        /// </summary>
        public int HandicapType { get; set; }

        /// <summary>
        /// 中奖状态 0 等待开奖、1 已中奖、2 取消 返还、3 走盘返还、4 赢半、5输半、6 未中奖
        /// </summary>
        public int WinStatus { get; set; }

        /// <summary>
        /// 订单状态  1 待定、2 成功、3 失败、4 事件挂起、5 事件挂起取消、6 操盘挂起、7 操盘挂起取消、8 订单取消
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 奖金
        /// </summary>
        public double Bonus { get; set; }

        /// <summary>
        /// 竞猜类型 0 篮球、1 足球、99 电竞、-9自主玩法...
        /// </summary>
        public string SportType { get; set; }

        /// <summary>
        /// 竞猜类型图片地址
        /// </summary>
        public string IconUrl { get; set; }

        /// <summary>
        /// 投注时间
        /// </summary>
        public string BetTime { get; set; }

        /// <summary>
        /// 中奖状态变更时间
        /// </summary>
        public string WinStatusTime { get; set; }

        /// <summary>
        /// 玩法名称
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 赛果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 订单时间列表
        /// </summary>
        public List<HistoryStatus> HistoryStatusList { get; set; }
    }
    public class HistoryStatus
    {
        /// <summary>
        /// 订单状态  1 待定、2 成功、3 失败、4 事件挂起
        /// 5 事件挂起取消、6 操盘挂起、7 操盘挂起取消、8 订单取消、10 开奖时间、10 未中奖、11 已中奖
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 时间  
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 订单变更时的备注
        /// </summary>
        public string Comment { get; set; }
    }
}
