using System;
using System.Web;
using MrCMS.Batching;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForBatchExecution : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return
                request.RequestContext.HttpContext.Request.Url.AbsolutePath.TrimStart('/')
                    .StartsWith(BatchExecutionController.BaseURL, StringComparison.OrdinalIgnoreCase);
        }
    }
}