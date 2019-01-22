using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ACL;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers
{
    public static class ACLHelper
    {
        public static bool CanAccess<T>(this IHtmlHelper html, string operation) where T : ACLRule, new()
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return html.CanAccess<T>(currentUser, operation);
        }

        public static bool CanAccess<T>(this IHtmlHelper html, User user, string operation) where T : ACLRule, new()
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return user != null && accessChecker.CanAccess<T>(operation, user);
        }

        public static bool CanAccess(this IHtmlHelper html, [AspMvcController]string controllerName, [AspMvcAction]string actionName)
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return html.CanAccess(currentUser, controllerName, actionName);
        }

        public static bool CanAccess(this IHtmlHelper html, User user, [AspMvcController]string controllerName, [AspMvcAction]string actionName)
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return user != null && accessChecker.CanAccess(controllerName, actionName, user);
        }
    }

}