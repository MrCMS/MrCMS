using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Areas.Admin.Filters
{
    public class AdminAuthFilter : IAsyncAuthorizationFilter
    {
        private readonly IAccessChecker _accessChecker;

        public AdminAuthFilter(IAccessChecker accessChecker)
        {
            _accessChecker = accessChecker;
        }


        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.IsAdminRequest())
                return;

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (!await _accessChecker.CanAccess(actionDescriptor))
                context.Result = new ForbidResult();
        }
    }
}
