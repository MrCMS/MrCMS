using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using MrCMS.Settings;
using MrCMS.Website.Caching;

namespace MrCMS.Website.Routing
{
    public class FileNotFoundHandler : IMrCMSRouteHandler
    {
        private readonly SiteSettings _siteSettings;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;
        private readonly ICacheWrapper _cacheWrapper;

        public FileNotFoundHandler(SiteSettings siteSettings, IMrCMSRoutingErrorHandler errorHandler, ICacheWrapper cacheWrapper)
        {
            _siteSettings = siteSettings;
            _errorHandler = errorHandler;
            _cacheWrapper = cacheWrapper;
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
                _cacheWrapper.Add(GetMissingFileCacheKey(context.HttpContext), new object(),
                    DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), Cache.NoSlidingExpiration,
                    CacheItemPriority.AboveNormal);
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.TrySkipIisCustomErrors = true;
                context.HttpContext.ApplicationInstance.CompleteRequest();
                return true;
            }
            return false;
        }

        public static string GetMissingFileCacheKey(HttpContextBase context)
        {
            return "MissingFile." + context.Request.Url.AbsolutePath;
        }

        public int Priority { get { return 10; } }
    }
}