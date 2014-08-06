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

            context.HttpContext.Response.RedirectPermanent("~/" + webpage.LiveUrlSegment);
            return false;
        }

        public int Priority { get { return 10000; } }
    }
}