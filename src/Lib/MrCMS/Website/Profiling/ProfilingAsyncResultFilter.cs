using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ProfilingAsyncResultFilter<TResultFilter> : IAsyncResultFilter where TResultFilter : IAsyncResultFilter
    {
        private readonly IServiceProvider _provider;

        public ProfilingAsyncResultFilter(IServiceProvider provider)
        {
            _provider = provider;
        }
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var type = typeof(TResultFilter);
            IAsyncResultFilter filter = _provider.GetService(type) as IAsyncResultFilter;

            using (MiniProfiler.Current.Step($"Profiling filter: {type.Name}"))
                return filter.OnResultExecutionAsync(context, next);
        }
    }
}