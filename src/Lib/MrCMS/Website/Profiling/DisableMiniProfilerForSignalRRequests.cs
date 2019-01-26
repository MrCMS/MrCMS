using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;

namespace MrCMS.Website.Profiling
{
    public class DisableMiniProfilerForSignalRRequests : IReasonToDisableMiniProfiler
    {
        public bool ShouldDisableFor(HttpRequest request)
        {
            var appContext = request.HttpContext.RequestServices.GetRequiredService<MrCMSAppContext>();
            return appContext.SignalRHubs.Values.Any(s =>
                request.Path.Value.StartsWith(s, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}