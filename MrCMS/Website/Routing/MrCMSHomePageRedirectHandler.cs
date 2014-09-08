using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Routing
{
    public class MrCMSHomePageRedirectHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly IGetHomePage _homePage;

        public MrCMSHomePageRedirectHandler(IGetWebpageForRequest getWebpageForRequest, IGetHomePage homePage)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _homePage = homePage;
        }

        public int Priority { get { return 9700; } }
        public bool Handle(RequestContext context)
        {
            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
                return false;
            if (webpage == _homePage.Get() && context.HttpContext.Request.Url.AbsolutePath != "/")
            {
                context.HttpContext.Response.Redirect("~/");
                return true;
            }
            return false;
        }
    }
}