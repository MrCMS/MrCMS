using System.Web;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public class MrCMSDisallowedHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;

        public MrCMSDisallowedHandler(IGetWebpageForRequest getWebpageForRequest, IMrCMSRoutingErrorHandler errorHandler)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _errorHandler = errorHandler;
        }

        public int Priority { get { return 1010; } }
        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (!webpage.IsAllowed(CurrentRequestData.CurrentUser))
            {
                string message = string.Format("Not allowed to view {0}", context.RouteData.Values["data"]);
                var code = CurrentRequestData.CurrentUser != null ? 403 : 401;
                _errorHandler.HandleError(context, code, new HttpException(code, message));
                return true;
            }
            return false;
        }
    }
}