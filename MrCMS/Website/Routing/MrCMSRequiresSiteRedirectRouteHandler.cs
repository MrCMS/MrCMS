using System;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public class MrCMSRequiresSiteRedirectRouteHandler : IMrCMSRouteHandler
    {
        private readonly IGetWebpageForRequest _getWebpageForRequest;

        public MrCMSRequiresSiteRedirectRouteHandler(IGetWebpageForRequest getWebpageForRequest)
        {
            _getWebpageForRequest = getWebpageForRequest;
        }

        public int Priority { get { return 9850; } }
        public bool Handle(RequestContext context)
        {
            var url = context.HttpContext.Request.Url;
            if (url != null)
            {
                Webpage webpage = _getWebpageForRequest.Get(context);
                if (webpage == null)
                    return false;
                var scheme = url.Scheme;
                var authority = url.Authority;
                var baseUrl = webpage.Site.BaseUrl;
                var stagingUrl = webpage.Site.StagingUrl;
                if (!context.HttpContext.Request.IsLocal && (!authority.Equals(baseUrl, StringComparison.InvariantCultureIgnoreCase) && !authority.Equals(stagingUrl, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var redirectUrl = url.ToString().Replace(scheme + "://" + authority, scheme + "://" + baseUrl);
                    context.HttpContext.Response.Redirect(redirectUrl);
                    return true;
                }
            }
            return false;
        }
    }
}