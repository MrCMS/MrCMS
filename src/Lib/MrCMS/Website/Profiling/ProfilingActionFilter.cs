using System;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ProfilingActionFilter<TActionFilter> : IActionFilter where TActionFilter : IActionFilter
    {
        private readonly IActionFilter _innerFilter;
        private readonly Type _type;

        public ProfilingActionFilter(IServiceProvider provider)
        {
            _type = typeof(TActionFilter);
            _innerFilter = provider.GetService(_type) as IActionFilter;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            using (MiniProfiler.Current.Step($"Profiling filter: {_type.Name}"))
            {
                _innerFilter.OnActionExecuting(context);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            using (MiniProfiler.Current.Step($"Profiling filter: {_type.Name}"))
            {
                _innerFilter.OnActionExecuted(context);
            }
        }
    }
}