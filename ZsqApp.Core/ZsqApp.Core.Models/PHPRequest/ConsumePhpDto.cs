using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class ConsumePhpDto
    {
        public string GameKey { get; set; }

        public string GameSetId { get; set; }

        public string OrderId { get; set; }

        public string UserId { get; set; }

        public Double Amount { get; set; }

        public long OperateTime { get; set; }

        public string Description { get; set; }
    }
}
