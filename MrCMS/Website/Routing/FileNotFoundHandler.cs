using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using System.Web.Mvc.Html;
using System.Web.Routing;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website.Caching;
using MrCMS.Website.Controllers;

namespace MrCMS.Website.Routing
{
    public class FileNotFoundHandler : IMrCMSRouteHandler
    {
        private const string ControllerName = "Error";
        private const string ActionName = "FileNotFound";
        private readonly WebExtensionsToRoute _webExtensions;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly IControllerManager _controllerManager;

        public FileNotFoundHandler(WebExtensionsToRoute webExtensions, IMrCMSRoutingErrorHandler errorHandler, ICacheWrapper cacheWrapper, IControllerManager controllerManager)
        {
            _webExtensions = webExtensions;
            _errorHandler = errorHandler;
            _cacheWrapper = cacheWrapper;
            _controllerManager = controllerManager;
        }

        public bool Handle(RequestContext context)
        {
            var path = context.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);

            if (_webExtensions.Get.Any(x => x == extension))
            {
                _errorHandler.HandleError(context, 404,
                    new HttpException(404, string.Format("Cannot find {0}", context.RouteData.Values["data"])));
                return true;
            }
            if (!string.IsNullOrWhiteSpace(extension))
            {

                var controller =
                    _controllerManager.ControllerFactory.CreateController(context, ControllerName) as ErrorController;
                controller.ControllerContext = new ControllerContext(context, controller)
                {
                    RouteData = context.RouteData
                };

                var routeValueDictionary = new RouteValueDictionary();
                routeValueDictionary["controller"] = ControllerName;
                routeValueDictionary["action"] = ActionName;
                routeValueDictionary["url"] = context.HttpContext.Request.Url;
                controller.RouteData.Values.Merge(routeValueDictionary);
                var result = controller.GetHtmlHelper().Action(ActionName, ControllerName, routeValueDictionary).ToString();

                _cacheWrapper.Add(GetMissingFileCacheKey(context.HttpContext.Request), result,
                    DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), Cache.NoSlidingExpiration,
                    CacheItemPriority.AboveNormal);

                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 404;
                context.HttpContext.Response.TrySkipIisCustomErrors = true;
                context.HttpContext.Response.Write(result);
                context.HttpContext.ApplicationInstance.CompleteRequest();

                return true;
            }
            return false;
        }

        public static string GetMissingFileCacheKey(HttpRequestBase request)
        {
            return "MissingFile." + request.Url.AbsolutePath;
        }

        public int Priority { get { return 10; } }
    }
}