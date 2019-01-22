using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Profiling
{
    public class ActionProfilingFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var description = context.ActionDescriptor is ControllerActionDescriptor descriptor
                ? $"Executing action: {descriptor.ControllerName} - {descriptor.ActionName}"
                : "Executing action";
            using (MiniProfiler.Current.Step(description))
            {
                return next();
            }
        }
    }
}