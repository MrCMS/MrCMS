using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public class MrCMSHomePageRedirectHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;

        public MrCMSHomePageRedirectHandler(IGetWebpageForRequest getWebpageForRequest)
        {
            _getWebpageForRequest = getWebpageForRequest;
        }

        public int Priority { get { return 1020; } }
        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
                return false;
            if (webpage.LiveUrlSegment != webpage.UrlSegment && context.HttpContext.Request.Url.AbsolutePath != "/")
            {
                context.HttpContext.Response.Redirect("~/" + webpage.LiveUrlSegment);
                return true;
            }
            return false;
        }
    }
}