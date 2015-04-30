using System.Web;

namespace MrCMS.Website.Profiling
{
    public interface IReasonToDisableMiniProfiler
    {
        bool ShouldDisableFor(HttpRequest request);
    }
}