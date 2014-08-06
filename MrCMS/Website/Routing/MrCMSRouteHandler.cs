using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class MrCMSRouteHandler : MvcRouteHandler
    {
        protected override IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return CurrentRequestData.DatabaseIsInstalled
                ? (IHttpHandler) MrCMSApplication.Get<MrCMSHttpHandler>()
                : new NotInstalledHandler();
        }
    }
}