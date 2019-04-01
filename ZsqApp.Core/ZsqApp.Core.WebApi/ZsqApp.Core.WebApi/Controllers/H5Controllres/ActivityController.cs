using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Interfaces.Activity;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.Interfaces.User;
using ZsqApp.Core.ViewModel.Activity;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Controllers.H5Controllres
{
    /// <summary>
    /// 活动相关
    /// </summary>
    [Produces("application/json")]
    [EnableCors("any")]
    [Route("api/activity")]
    public class ActivityController : Controller
    {
        /// <summary>
        /// ISystems
        /// </summary>
        protected readonly ISystems _sys;

        /// <summary>
        /// user token
        /// </summary>
        protected readonly IToken _token;

        /// <summary>
        /// activity
        /// </summary>
        protected readonly IActivity _activity;


        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:ZsqApp.Core.WebApi.Controllers.H5Controllres.ActivityController"/> class.
        /// </summary>
        /// <param name="systems">Systems.</param>
        /// <param name="token">Systems.</param>
        /// <param name="activity">Systems.</param>
        public ActivityController(ISystems systems, IToken token, IActivity activity)
        {
            _sys = systems;
            _token = token;
            _activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        /// <summary>
        /// 获取活动列表
        /// author：陶林辉
        /// </summary>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("activityList")]
        public async Task<H5ResponseViewModel<ActivityViewModel>> ActivityList([FromBody]H5RequestViewModel Parameters)
        {
            var sysCode = _sys.CheckParameters(Parameters.data, "ActivityList");
            H5ResponseViewModel<ActivityViewModel> response = null;
            ActivityViewModel result = new ActivityViewModel
            {
                data = new List<Activity>()
            };
            if (sysCode == SysCode.Ok)
            {
                string token = Parameters.data.token;
                bool isNo = _token.VerifyToken(token);
                if (isNo)
                {
                    if (((string)Parameters.data.channel).IsNotBlank())
                    {
                        result.data = await _activity.GetActivityListAsync((string)Parameters.data.channel);
                        result.count = result.data.Count();
                    }
                    else
                    {
                        result.data = await _activity.GetActivityListAsync();
                        result.count = result.data.Count();
                    }
                }
                else
                {
                    sysCode = SysCode.TokenLose;
                }
            }
            response = new H5ResponseViewModel<ActivityViewModel>(sysCode, result.count == 0 ? null : result);
            return response;
        }
    }
}
