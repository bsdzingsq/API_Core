using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.WebApi.Model
{
    /// <summary>
    /// 对外游戏投注，派奖。撤销标准请求
    /// </summary>
    public class GameRequestViweModel
    {

        public string appKey { get; set; }

        public string requestId { get; set; }

        public string timestamp { get; set; }

        public string nonce { get; set; }

        public string language { get; set; }

        public string sign { get; set; }

        public string signType { get; set; }

        public dynamic data { get; set; }

        public Page page { get; set; }
    }
}
