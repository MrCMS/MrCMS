using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;

namespace MrCMS.Website.Filters
{
    public class ForceImmediateLuceneUpdateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // only do this with local file system due to unpredictability of remote file storage
            var serviceProvider = filterContext.HttpContext.RequestServices;
            if (serviceProvider.GetRequiredService<IFileSystem>() is FileSystem)
            {
                serviceProvider.GetRequiredService<IEndRequestTaskManager>().AddTask(new ExecuteLuceneTasks());
            }
        }
    }
}