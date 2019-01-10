using Microsoft.AspNetCore.Http;
using MrCMS.Services;
using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public class TrimTrailingSlashMiddleware : IMiddleware
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetWebpageForPath _getWebpageForPath;

        public TrimTrailingSlashMiddleware(IGetCurrentPage getCurrentPage, IGetWebpageForPath getWebpageForPath)
        {
            _getCurrentPage = getCurrentPage;
            _getWebpageForPath = getWebpageForPath;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currentPage = _getCurrentPage.GetPage();
            if (currentPage != null)
            {
                return next(context);
            }

            if (context.Request.Path.HasValue && context.Request.Path.Value.EndsWith('/'))
            {
                var webpage = _getWebpageForPath.GetWebpage(context.Request.Path.Value.TrimEnd('/'));
                if (webpage != null)
                {
                    context.Response.Redirect("/" + webpage.UrlSegment, true);
                    return Task.CompletedTask;
                }

            }
            return next(context);
        }
    }
}