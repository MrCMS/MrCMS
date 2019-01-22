using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website
{
    public class EndRequestHandlerFilter : IAsyncResourceFilter
    {
        private readonly IEndRequestTaskManager _manager;

        public EndRequestHandlerFilter(IEndRequestTaskManager manager)
        {
            _manager = manager;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            await next();
            _manager.ExecuteTasks();
        }
    }
}