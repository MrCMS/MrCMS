using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Tasks;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForTaskExecution : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return
                request.HttpContext.Request.Path.Value.TrimStart('/')
                    .StartsWith(TaskExecutionController.ExecutePendingTasksURL, StringComparison.OrdinalIgnoreCase);
        }
    }
}