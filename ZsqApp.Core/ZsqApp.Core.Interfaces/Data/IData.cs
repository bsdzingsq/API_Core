using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZsqApp.Core.Interfaces.Data
{
    public interface IData
    {
        /// <summary>
        /// 拉取老的用户到新库。需要关闭自增
        /// author：陶林辉
        /// </summary>
        Task<bool> InsertData();
    }
}
