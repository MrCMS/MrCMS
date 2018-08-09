using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ACL;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers
{
    public static class ACLHelper
    {
        public static bool CanAccess<T>(this IHtmlHelper html, string operation, string type = null) where T : ACLRule, new()
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return currentUser != null && accessChecker.CanAccess<T>(operation, currentUser);
                   //new T().CanAccess(currentUser, operation, type);
        }
    }

}