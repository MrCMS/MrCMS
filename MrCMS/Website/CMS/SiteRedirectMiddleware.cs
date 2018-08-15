using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class SiteRedirectMiddleware : IMiddleware
    {
        private readonly IGetCurrentPage _getCurrentPage;

        public SiteRedirectMiddleware(IGetCurrentPage getCurrentPage)
        {
            _getCurrentPage = getCurrentPage;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var webpage = _getCurrentPage.GetPage();
            if (webpage != null)
            {
                var scheme = context.Request.Scheme;
                var authority = context.Request.Host;
                var baseUrl = new HostString(webpage.Site.BaseUrl);
                var stagingUrl = new HostString(webpage.Site.StagingUrl);
                if (!context.Request.IsLocal() && (!authority.Equals(baseUrl) && !authority.Equals(stagingUrl)))
                {
                    var uriBuilder = new UriBuilder(scheme, baseUrl.Host, baseUrl.Port ?? 80, context.Request.Path,
                        context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null);
                    var location = uriBuilder.ToString();
                    context.Response.Redirect(location);
                    return Task.CompletedTask;
                }

            }

            return next(context);
        }
    }
}