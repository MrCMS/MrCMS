using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ProfilingAsyncAuthorizationFilter<TActionFilter> : IAsyncAuthorizationFilter where TActionFilter : IAsyncAuthorizationFilter
    {
        private readonly IServiceProvider _provider;

        public ProfilingAsyncAuthorizationFilter(IServiceProvider provider)
        {
            _provider = provider;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var type = typeof(TActionFilter);
            var filter = _provider.GetService(type) as IAsyncAuthorizationFilter;

            using (MiniProfiler.Current.Step($"Profiling filter: {type.Name}"))
                return filter.OnAuthorizationAsync(context);
        }
    }
}