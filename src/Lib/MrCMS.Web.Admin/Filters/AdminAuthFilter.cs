using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Filters
{
    public class AdminAuthFilter : IAuthorizationFilter
    {
        private readonly IAccessChecker _accessChecker;

        public AdminAuthFilter(IAccessChecker accessChecker)
        {
            _accessChecker = accessChecker;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.IsAdminRequest())
                return;

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (!_accessChecker.CanAccess(actionDescriptor))
                context.Result = new ForbidResult();
        }
    }
}
