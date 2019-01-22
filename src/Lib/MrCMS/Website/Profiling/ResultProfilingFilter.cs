using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ResultProfilingFilter : IAsyncResultFilter
    {
        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            using (MiniProfiler.Current.Step("Executing result"))
            {
                return next();
            }
        }
    }
}