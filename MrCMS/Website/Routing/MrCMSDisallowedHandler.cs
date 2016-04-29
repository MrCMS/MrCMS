using System.Web;
using System.Web.Routing;
using MrCMS.Services;

namespace MrCMS.Website.Routing
{
    public class MrCMSDisallowedHandler : IMrCMSRouteHandler
    {
        private readonly IMrCMSRoutingErrorHandler _errorHandler;
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IUserUIPermissionsService _userUIPermissionsService;

        public MrCMSDisallowedHandler(IGetWebpageForRequest getWebpageForRequest, IMrCMSRoutingErrorHandler errorHandler,
            IUserUIPermissionsService userUIPermissionsService)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _errorHandler = errorHandler;
            _userUIPermissionsService = userUIPermissionsService;
        }

        public int Priority
        {
            get { return 9650; }
        }

        public bool Handle(RequestContext context)
        {
            var webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
                return false;
            if (!_userUIPermissionsService.IsCurrentUserAllowed(webpage))
            {
                var message = string.Format("Not allowed to view {0}", context.RouteData.Values["data"]);
                var code = CurrentRequestData.CurrentUser != null ? 403 : 401;
                _errorHandler.HandleError(context, code, new HttpException(code, message));
                return true;
            }
            return false;
        }
    }
}