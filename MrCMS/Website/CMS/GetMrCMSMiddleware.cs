using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Apps;
using MrCMS.Logging;
using MrCMS.Services;
using MrCMS.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public class GetMrCMSMiddleware : IGetMrCMSMiddleware
    {
        public IEnumerable<MiddlewareInfo> GetSortedMiddleware(MrCMSAppContext appContext)
        {
            var sortedMiddleware = new List<MiddlewareInfo>
            {
                new MiddlewareInfo
                {
                    Registration = builder => builder.UseMiddleware<MrCMSLoggingMiddleware>(),
                    Order = int.MaxValue
                },
                new MiddlewareInfo
                {
                    Registration = builder => builder.UseMiddleware<CurrentSiteSettingsMiddleware>(),
                    Order = int.MaxValue - 1
                },
                new MiddlewareInfo
                {
                    Registration = builder => builder.UseMiddleware<CurrentWebpageMiddleware>(),
                    Order = int.MaxValue - 1
                },
                new MiddlewareInfo
                {
                    Registration = builder => builder.UseMiddleware<SiteRedirectMiddleware>(),
                    Order = 9850
                },
                new MiddlewareInfo
                {
                    Registration = builder => builder.UseMiddleware<WebpageDisallowedMiddleware>(),
                    Order = 9650
                },
                new MiddlewareInfo
                {
                    Registration = builder =>
                        builder.UseWhen(
                            context => context.RequestServices.GetRequiredService<SiteSettings>().SSLEverywhere,
                            app => app.UseHttpsRedirection()),
                    Order = 10000
                },
            };


            sortedMiddleware.AddRange(appContext.Middlewares);
            return sortedMiddleware.OrderByDescending(x => x.Order);
        }
    }

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