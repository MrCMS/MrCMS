using System.Threading.Tasks;
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
        public static async Task<bool> CanAccess(this IHtmlHelper html, [AspMvcController]string controllerName, [AspMvcAction]string actionName, string method = GetAdminActionDescriptor.DefaultMethod)
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return await html.CanAccess(currentUser, controllerName, actionName, method);
        }

        public static async Task<bool> CanAccess(this IHtmlHelper html, User user, [AspMvcController]string controllerName, [AspMvcAction]string actionName, string method = GetAdminActionDescriptor.DefaultMethod)
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            var getAdminActionDescriptor = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetAdminActionDescriptor>();
            return user != null && await accessChecker.CanAccess(getAdminActionDescriptor.GetDescriptor(controllerName,actionName,method), user);
        }
    }
}