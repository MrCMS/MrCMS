using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;

namespace MrCMS.Web.Apps.Core.Filters
{
    public class PasswordAuthFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // todo - refactor this to be picked up a different way
            var webpage = context.ActionArguments.Values.OfType<Webpage>().FirstOrDefault();

            if (webpage == null)
            {
                await next();
                return;
            }

            var serviceProvider = context.HttpContext.RequestServices;
            var canAccess = await serviceProvider.GetRequiredService<IPasswordProtectedPageChecker>()
                .CanAccessPage(webpage, context.HttpContext.Request.Cookies);
            if (canAccess)
            {
                await next();
                return;
            }

            context.Result = await serviceProvider.GetRequiredService<IUniquePageService>()
                .RedirectTo<WebpagePasswordPage>(new { lockedPage = webpage.Id });
        }
    }
}