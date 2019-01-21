using System.Web;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Website.Profiling
{
    public interface IReasonToEnableMiniProfiler
    {
        bool ShouldEnableFor(HttpRequest request);
    }
}