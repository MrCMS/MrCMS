using System;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ProfilingAuthorizationFilter<TActionFilter> : IAuthorizationFilter where TActionFilter : IAuthorizationFilter
    {
        private readonly IServiceProvider _provider;

        public ProfilingAuthorizationFilter(IServiceProvider provider)
        {
            _provider = provider;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var type = typeof(TActionFilter);
            var filter = _provider.GetService(type) as IAuthorizationFilter;

            using (MiniProfiler.Current.Step($"Profiling filter: {type.Name}"))
                filter.OnAuthorization(context);
        }
    }
   }