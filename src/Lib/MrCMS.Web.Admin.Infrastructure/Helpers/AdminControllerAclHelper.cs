using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.Routing;
using MrCMS.Website.Auth;

namespace MrCMS.Web.Admin.Infrastructure.Helpers
{
    public static class AdminControllerAclHelper
    {
        public static bool CanAccess(this IHtmlHelper html, [AspMvcController]string controllerName, [AspMvcAction]string actionName, string method = GetAdminActionDescriptor.DefaultMethod)
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return html.CanAccess(currentUser, controllerName, actionName, method);
        }

        public static bool CanAccess(this IHtmlHelper html, User user, [AspMvcController]string controllerName, [AspMvcAction]string actionName, string method = GetAdminActionDescriptor.DefaultMethod)
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            var getAdminActionDescriptor = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetAdminActionDescriptor>();
            return user != null && accessChecker.CanAccess(getAdminActionDescriptor.GetDescriptor(controllerName,actionName,method), user);
        }
    }
}