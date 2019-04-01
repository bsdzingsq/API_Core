using System;
using System.Collections.Generic;
using System.Text;

namespace ZsqApp.Core.Models.PHPRequest
{
    public class ConsumeDto
    {
        public string fuserId { get; set; }
        public string forderId { get; set; }
        public Double amount { get; set; }
        public string description { get; set; }
    }
}
