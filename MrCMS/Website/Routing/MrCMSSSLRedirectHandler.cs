using System.Web.Routing;
using MrCMS.Entities.Documents.Web;
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

        public int Priority { get { return 1041; } }
        public bool Handle(RequestContext context)
        {
            var url = context.HttpContext.Request.Url;
            var scheme = url.Scheme;

            Webpage webpage = _getWebpageForRequest.Get(context);
            if (webpage == null)
                return false;
            if (webpage.RequiresSSL && scheme != "https" && _siteSettings.SiteIsLive)
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                context.HttpContext.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            if (!webpage.RequiresSSL && scheme != "http")
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "http://");
                context.HttpContext.Response.RedirectPermanent(redirectUrl);
                return true;
            }
            return false;
        }
    }
}