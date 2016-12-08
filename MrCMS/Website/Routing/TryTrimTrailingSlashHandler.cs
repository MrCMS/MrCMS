using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class TryTrimTrailingSlashHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _webpageForRequest;

        public TryTrimTrailingSlashHandler(IGetWebpageForRequest webpageForRequest)
        {
            _webpageForRequest = webpageForRequest;
        }

        public int Priority { get { return 501; } }
        public bool Handle(RequestContext context)
        {
            var path = context.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
                return false;
            var webpage = _webpageForRequest.Get(context, Convert.ToString(context.RouteData.Values["data"]).TrimEnd('/'));
            if (webpage == null)
                return false;

            context.HttpContext.Response.RedirectPermanent("~/" + webpage.LiveUrlSegment);
            return true;
        }
    }
}