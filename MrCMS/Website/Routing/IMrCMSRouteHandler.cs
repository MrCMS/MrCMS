using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public interface IMrCMSRouteHandler
    {
        int Priority { get; }
        bool Handle(RequestContext context);
    }
}