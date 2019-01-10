using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class HomePageRedirectMiddleware : IMiddleware
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetHomePage _getHomePage;

        public HomePageRedirectMiddleware(IGetCurrentPage getCurrentPage, IGetHomePage getHomePage)
        {
            _getCurrentPage = getCurrentPage;
            _getHomePage = getHomePage;
        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path == "/")
                return next(context);
            var webpage = _getCurrentPage.GetPage();
            var homepage = _getHomePage.Get();
            if (webpage != null && homepage != null && webpage.Id == homepage.Id)
            {
                context.Response.Redirect("/", true);
            }

            return next(context);
        }
    }
}