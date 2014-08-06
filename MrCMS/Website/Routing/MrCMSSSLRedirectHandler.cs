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

        public int Priority { get { return 1040; } }
        public bool Handle(RequestContext context)
        {
            var url = context.HttpContext.Request.Url;
            var scheme = url.Scheme;
            if (CurrentRequestData.CurrentUserIsAdmin && _siteSettings.SSLAdmin && _siteSettings.SiteIsLive && !context.HttpContext.Request.IsLocal)
            {
                if (scheme == "http")
                {
                    var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                    context.HttpContext.Response.Redirect(redirectUrl);
                    return true;
                }
                return false;
            }
            if (CurrentRequestData.CurrentUserIsAdmin && scheme == "http" && _siteSettings.SSLAdmin && _siteSettings.SiteIsLive && !context.HttpContext.Request.IsLocal)
            {
                var redirectUrl = url.ToString().Replace(scheme + "://", "https://");
                context.HttpContext.Response.Redirect(redirectUrl);
                return true;
            }
            Webpage webpage = _getWebpageForRequest.Get(context);
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