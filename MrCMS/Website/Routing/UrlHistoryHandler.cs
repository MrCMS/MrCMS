using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class UrlHistoryHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpage;
        private readonly IGetWebpageByUrlHistoryForRequest _getWebpageByUrlHistory;

        public UrlHistoryHandler(IGetWebpageForRequest getWebpage, IGetWebpageByUrlHistoryForRequest getWebpageByUrlHistory)
        {
            _getWebpage = getWebpage;
            _getWebpageByUrlHistory = getWebpageByUrlHistory;
        }

        public bool Handle(RequestContext context)
        {
            if (_getWebpage.Get(context) != null)
                return false;

            var webpage = _getWebpageByUrlHistory.Get(context);
            if (webpage == null)
                return false;

            var queryStringExist = context.HttpContext.Request.QueryString.Count > 0;
            var redirectUrl = "~/" + webpage.LiveUrlSegment;
            if (queryStringExist)
                redirectUrl += "?" + context.HttpContext.Request.QueryString;
            context.HttpContext.Response.RedirectPermanent(redirectUrl);
            return false;
        }

        public int Priority { get { return 10000; } }
    }
}