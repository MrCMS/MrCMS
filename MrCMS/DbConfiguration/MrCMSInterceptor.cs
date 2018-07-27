using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public class MrCMSInterceptor : EmptyInterceptor
    {
        public HttpContext Context => ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
        public IServiceProvider ServiceProvider { get; }

        public MrCMSInterceptor(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}