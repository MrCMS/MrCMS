using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class FileNotFoundHandler : IMrCMSRouteHandler
    {
        private readonly SiteSettings _siteSettings;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;

        public FileNotFoundHandler(SiteSettings siteSettings, IMrCMSRoutingErrorHandler errorHandler)
        {
            _siteSettings = siteSettings;
            _errorHandler = errorHandler;
        }

        public bool Handle(RequestContext context)
        {
            var path = context.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);

            if (_siteSettings.WebExtensionsToRoute.Any(x => x == extension))
            {
                _errorHandler.HandleError(context, 404,
                    new HttpException(404, string.Format("Cannot find {0}", context.RouteData.Values["data"])));
                return true;
            }
            if (!string.IsNullOrWhiteSpace(extension))
            {
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.End();
                return true;
            }
            return false;
        }

        public int Priority { get { return 10; } }
    }
}