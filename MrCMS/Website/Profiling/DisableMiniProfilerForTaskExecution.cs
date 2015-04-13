using System;
using System.Web;
using MrCMS.Tasks;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForTaskExecution : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return
                request.RequestContext.HttpContext.Request.Url.AbsolutePath.TrimStart('/')
                    .StartsWith(TaskExecutionController.ExecutePendingTasksURL, StringComparison.OrdinalIgnoreCase);
        }
    }
}