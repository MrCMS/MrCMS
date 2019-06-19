using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services;

namespace MrCMS.Web.Apps.Core.Filters
{
    public class PasswordAuthFilter : IActionFilter
    {
        private readonly IPasswordProtectedPageChecker _checker;
        private readonly IUniquePageService _uniquePageService;

        public PasswordAuthFilter(IPasswordProtectedPageChecker checker, IUniquePageService uniquePageService)
        {
            _checker = checker;
            _uniquePageService = uniquePageService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.ContainsKey("page"))
            {
                return;
            }

            if (!(context.ActionArguments["page"] is Webpage page))
            {
                return;
            }

            var canAccess = _checker.CanAccessPage(page, context.HttpContext.Request.Cookies);
            if (canAccess)
            {
                return;
            }

            context.Result = _uniquePageService.RedirectTo<WebpagePasswordPage>(new {lockedPage = page.Id});
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}