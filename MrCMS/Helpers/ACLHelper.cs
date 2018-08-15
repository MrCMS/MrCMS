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
        public static bool CanAccess<T>(this IHtmlHelper html, string operation, string type = null) where T : ACLRule, new()
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return html.CanAccess<T>(currentUser, operation, type);
        }

        public static bool CanAccess<T>(this IHtmlHelper html, User user, string operation, string type = null) where T : ACLRule, new()
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return user != null && accessChecker.CanAccess<T>(operation, user);
        }
    }

}