using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.Interfaces.System;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.WebApi.Model
{
    public class UnknownViewModel<T>
    {
        protected readonly ISystems _sys; /**/
        public UnknownViewModel(SysCode sysCode, ISystems sys, T data)
        {
            _sys = sys;
            Code = (int)sysCode;
            Message = EnumExtention.GetDescription(sysCode);
            if (data != null)
            {
                this.Data = data;
            }


        }
        public int Code { get; set; }

        public string Message { get; set; }

        public dynamic Data { get; set; }
    }
}
