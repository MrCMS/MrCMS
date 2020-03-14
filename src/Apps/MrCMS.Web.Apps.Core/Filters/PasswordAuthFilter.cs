using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;

namespace MrCMS.Web.Apps.Core.Filters
{
    public class PasswordAuthFilter : IAsyncActionFilter
    {
        private readonly IPasswordProtectedPageChecker _checker;
        private readonly IUniquePageService _uniquePageService;

        public PasswordAuthFilter(IPasswordProtectedPageChecker checker, IUniquePageService uniquePageService)
        {
            _checker = checker;
            _uniquePageService = uniquePageService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var webpage = context.ActionArguments.Values.OfType<Webpage>().FirstOrDefault();

            if (webpage == null)
            {
                await next();
                return;
            }

            var canAccess = await _checker.CanAccessPage(webpage, context.HttpContext.Request.Cookies);
            if (canAccess)
            {
                await next();
                return;
            }

            context.Result = await _uniquePageService.RedirectTo<WebpagePasswordPage>(new { lockedPage = webpage.Id });
        }
    }
}