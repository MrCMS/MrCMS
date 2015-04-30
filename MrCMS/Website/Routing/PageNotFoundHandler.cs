using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class PageNotFoundHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _webpageForRequest;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;

        public PageNotFoundHandler(IGetWebpageForRequest webpageForRequest, IMrCMSRoutingErrorHandler errorHandler)
        {
            _webpageForRequest = webpageForRequest;
            _errorHandler = errorHandler;
        }

        public int Priority { get { return 500; } }
        public bool Handle(RequestContext context)
        {
            var path = context.HttpContext.Request.Url.AbsolutePath;
            var extension = Path.GetExtension(path);
            if (!string.IsNullOrWhiteSpace(extension))
                return false;
            if (_webpageForRequest.Get(context) == null)
            {
                _errorHandler.HandleError(context, 404,
                    new HttpException(404, string.Format("Cannot find {0}", context.RouteData.Values["data"])));
                return true;
            }
            return false;
        }
    }
}