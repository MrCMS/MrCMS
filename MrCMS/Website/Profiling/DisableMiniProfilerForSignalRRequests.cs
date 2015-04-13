using System;
using System.Web;
using MrCMS.Helpers;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForSignalRRequests : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            return request.RawUrl.Contains("signalr", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}