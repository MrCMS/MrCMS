using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Batching;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForBatchExecution : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return
                request.HttpContext.Request.Path.Value.TrimStart('/')
                    .StartsWith(BatchExecutionController.BaseURL, StringComparison.OrdinalIgnoreCase);
        }
    }
}