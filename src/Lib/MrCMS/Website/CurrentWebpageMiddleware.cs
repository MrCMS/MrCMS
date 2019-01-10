using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Services;

namespace MrCMS.Website
{
    public class CurrentWebpageMiddleware : IMiddleware
    {
        private readonly IGetWebpageForPath _getWebpageForPath;
        private readonly ISetCurrentPage _setCurrentPage;

        public CurrentWebpageMiddleware(IGetWebpageForPath getWebpageForPath, ISetCurrentPage setCurrentPage)
        {
            _getWebpageForPath = getWebpageForPath;
            _setCurrentPage = setCurrentPage;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var webpage = _getWebpageForPath.GetWebpage(context.Request.Path);

            if (webpage != null)
            {
                _setCurrentPage.SetPage(webpage);
            }

            await next(context);
        }
    }
}