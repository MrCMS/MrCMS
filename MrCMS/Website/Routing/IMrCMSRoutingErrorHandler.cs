using System.Web;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public interface IMrCMSRoutingErrorHandler
    {
        void HandleError(RequestContext context, int code, HttpException exception);
    }
}