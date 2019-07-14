using Microsoft.AspNetCore.Http;
using MrCMS.Helpers;
using MrCMS.Services;
using System;
using System.Threading.Tasks;

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
                    var uri = new UriBuilder
                    {
                        Scheme = scheme,
                        Host = baseUrl.Host,
                        Path = context.Request.Path,
                        Query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                    };
                    if (baseUrl.Port != null && baseUrl.Port != 80 && baseUrl.Port != 443)
                    {
                        uri.Port = (int) baseUrl.Port;
                    }
                    var location = uri.ToString();
                    context.Response.Redirect(location);
                    return Task.CompletedTask;
                }

            }

            return next(context);
        }
    }
}