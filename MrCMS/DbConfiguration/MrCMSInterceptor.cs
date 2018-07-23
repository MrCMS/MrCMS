using System;
using Microsoft.AspNetCore.Http;
using NHibernate;

namespace MrCMS.DbConfiguration
{
    public class MrCMSInterceptor : EmptyInterceptor
    {
        public HttpContext Context { get; }
        public IServiceProvider ServiceProvider { get; }

        public MrCMSInterceptor(HttpContext context, IServiceProvider serviceProvider)
        {
            Context = context;
            ServiceProvider = serviceProvider;
        }
    }
}