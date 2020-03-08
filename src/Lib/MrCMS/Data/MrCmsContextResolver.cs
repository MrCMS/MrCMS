using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.DbConfiguration;

namespace MrCMS.Data
{
    public class MrCmsContextResolver : IMrCmsContextResolver
    {
        private readonly IServiceProvider _serviceProvider;
        public static Type Type => typeof(WebsiteContext);

        public MrCmsContextResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public DbContext Resolve() => _serviceProvider.GetRequiredService<WebsiteContext>();
    }
}