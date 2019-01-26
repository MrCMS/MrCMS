using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ProfilingAsyncActionFilter<TActionFilter> : IAsyncActionFilter where TActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _provider;

        public ProfilingAsyncActionFilter(IServiceProvider provider)
        {
            _provider = provider;
        }
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var type = typeof(TActionFilter);
            IAsyncActionFilter filter = _provider.GetService(type) as IAsyncActionFilter;

            using (MiniProfiler.Current.Step($"Profiling filter: {type.Name}"))
                return filter.OnActionExecutionAsync(context, next);
        }
    }
}