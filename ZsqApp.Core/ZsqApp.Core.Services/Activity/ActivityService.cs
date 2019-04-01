using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ZsqApp.Core.Entity;
using ZsqApp.Core.Entity.Activity;
using ZsqApp.Core.Interfaces.Activity;
using ZsqApp.Core.ViewModel.Activity;

namespace ZsqApp.Core.Services.Activity
{
    public class ActivityService : IActivity
    {

        /// <summary>
        /// app context
        /// </summary>
        protected readonly FunHaiNanContext _funHaiNanContext;

        /// <summary>
        /// aotumapper
        /// </summary>
        protected readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ZsqApp.Core.Services.Activity.ActivityService"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="mapper">Mapper.</param>
        public ActivityService(FunHaiNanContext context, IMapper mapper)
        {
            _funHaiNanContext = context;
            _mapper = mapper;

        }


        /// <summary>
        /// Gets the activity list asyn by channel.
        /// </summary>
        /// <returns>The activity list asyn.</returns>
        /// <param name="channel">Channel.</param>
        public async Task<List<ZsqApp.Core.ViewModel.Activity.Activity>> GetActivityListAsync(string channel)
        {
            var activityId = await _funHaiNanContext.Activity_Channel.Where(m => m.Channel == channel).Select(m => m.Activity_Id).ToListAsync();
            var list = await _funHaiNanContext.Activity.Where(m => activityId.Contains(m.Id) && m.Participation_Type_Id == 2 && m.Display == 1).ToListAsync();
            var result = _mapper.Map<List<ActivityEntity>, List<ZsqApp.Core.ViewModel.Activity.Activity>>(list);
            result.AddRange(await GetActivityListAsync());
            result = result.OrderByDescending(m => m.Sort).ToList();
            return result;


        }

        /// <summary>
        /// Gets the activity list asyn by channel.
        /// </summary>
        /// <returns>The activity list asyn.</returns>
        /// <param>Channel.</param>
        public async Task<List<ZsqApp.Core.ViewModel.Activity.Activity>> GetActivityListAsync()
        {
            var list = await _funHaiNanContext.Activity.Where(m => m.Participation_Type_Id == 1 && m.Display == 1).OrderByDescending(t => t.Sort).ToListAsync();
            return _mapper.Map<List<ActivityEntity>, List<ZsqApp.Core.ViewModel.Activity.Activity>>(list);
        }
    }
}
