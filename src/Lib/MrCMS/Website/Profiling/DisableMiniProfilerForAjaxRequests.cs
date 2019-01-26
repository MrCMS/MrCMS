using Microsoft.AspNetCore.Http;
using MrCMS.Helpers;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForAjaxRequests : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return request.HttpContext.Request.IsAjaxRequest();
        }
    }
}