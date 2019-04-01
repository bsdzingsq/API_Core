using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZsqApp.Core.Infrastructure.Utilities;
using ZsqApp.Core.Models.Headers;
using ZsqApp.Core.ViewModel.ErrCodeEnum;
using ZsqApp.Core.WebApi.Model;

namespace ZsqApp.Core.WebApi.Filters
{
    public class VisitMiddleware
    {
        readonly RequestDelegate _next;

        public VisitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           
            await _next(context);

        }
    }
}
