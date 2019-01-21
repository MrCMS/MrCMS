using System.Web;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Website.Profiling
{
    public interface IReasonToDisableMiniProfiler
    {
        bool ShouldDisableFor(HttpRequest request);
    }
}