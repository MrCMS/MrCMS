using System.Web;

namespace MrCMS.Website.Profiling
{
    public interface IReasonToEnableMiniProfiler
    {
        bool ShouldEnableFor(HttpRequest request);
    }
}