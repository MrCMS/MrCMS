using System.Web;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.Routing
{
    public class MrCMSDisallowedHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IMrCMSRoutingErrorHandler _errorHandler;
        private readonly IUserUIPermissionsService _userUIPermissionsService;

        public MrCMSDisallowedHandler(IGetWebpageForRequest getWebpageForRequest, IMrCMSRoutingErrorHandler errorHandler, IUserUIPermissionsService userUIPermissionsService)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _errorHandler = errorHandler;
            _userUIPermissionsService = userUIPermissionsService;
        }

        public int Priority { get { return 1010; } }
        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (!_userUIPermissionsService.IsCurrentUserAllowed(webpage))
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