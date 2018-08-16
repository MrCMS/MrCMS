using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Services;

namespace MrCMS.Website.CMS
{
    public class WebpageDisallowedMiddleware : IMiddleware
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IUserUIPermissionsService _userUIPermissionsService;
        private readonly IGetCurrentUser _getCurrentUser;

        public WebpageDisallowedMiddleware(IGetCurrentPage getCurrentPage, IUserUIPermissionsService userUIPermissionsService, IGetCurrentUser getCurrentUser)
        {
            _getCurrentPage = getCurrentPage;
            _userUIPermissionsService = userUIPermissionsService;
            _getCurrentUser = getCurrentUser;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var webpage = _getCurrentPage.GetPage();
            if (webpage == null || _userUIPermissionsService.IsCurrentUserAllowed(webpage))
            {
                await next(context);
                return;
            }

            var message = string.Format("Not allowed to view {0}", context.Request.Path);
            var code = _getCurrentUser.Get() != null ? 403 : 401;
            //return true;

            context.Response.StatusCode = code;
            await context.Response.WriteAsync(message);
            // TODO: improve quality of 403 and 401, maybe need to move up to MVC
        }
    }
}