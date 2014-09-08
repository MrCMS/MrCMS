using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Website.Routing
{
    public class MrCMSSSLRedirectHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;
        private readonly SiteSettings _siteSettings;

        public MrCMSSSLRedirectHandler(IGetWebpageForRequest getWebpageForRequest, SiteSettings siteSettings)
        {
            _getWebpageForRequest = getWebpageForRequest;
            _siteSettings = siteSettings;
        }

        public int Priority { get { return 9850; } }
        public bool Handle(RequestContext context)
        {
            var url = context.HttpContext.Request.Url;
            var scheme = url.Scheme;

            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
                return false;
            if (webpage.RequiresSSL(context.HttpContext.Request, _siteSettings) && scheme != "https")
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                context.HttpContext.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            if (!webpage.RequiresSSL(context.HttpContext.Request, _siteSettings) && scheme != "http")
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "http://");
                context.HttpContext.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            return false;
        }
    }
}