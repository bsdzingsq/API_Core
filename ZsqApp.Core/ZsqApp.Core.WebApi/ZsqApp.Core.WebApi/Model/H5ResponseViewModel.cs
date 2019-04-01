using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZsqApp.Core.Infrastructure.Extentions;
using ZsqApp.Core.ViewModel.ErrCodeEnum;

namespace ZsqApp.Core.WebApi.Model
{
    public class H5ResponseViewModel<T>
    {

        public H5ResponseViewModel(SysCode sysCode, T data)
        {
            Code = (int)sysCode;
            Message = EnumExtention.GetDescription(sysCode);
            this.data = data;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public dynamic data { get; set; }


    }
}
