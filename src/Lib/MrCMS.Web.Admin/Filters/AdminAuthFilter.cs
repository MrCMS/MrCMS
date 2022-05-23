using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Filters
{
    public class AdminAuthFilter : IAsyncAuthorizationFilter
    {
        // private readonly IAccessChecker _accessChecker;
        //
        // public AdminAuthFilter(IAccessChecker accessChecker)
        // {
        //     _accessChecker = accessChecker;
        // }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.IsAdminRequest())
                return;

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var accessChecker = context.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            var getCurrentUser = context.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>();
            if (!await accessChecker.CanAccess(actionDescriptor, await getCurrentUser.Get()))
                context.Result = new ForbidResult();
        }
    }
}