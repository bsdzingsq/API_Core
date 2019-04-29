using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class RechargesResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public orderRechargeList Data { get; set; }
    }
    public class orderRechargeList
    {
        public string orderIdStr { get; set; }
        public List<orderInfoList> orderInfo { get; set; }
    }
    public class orderInfoList
    {
        public string UserId { get; set; }
        public string OrderId { get; set; }
        public string PayType { get; set; }
    }

}
