using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ACL;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class ACLHelper
    {
        public static bool CanAccess<T>(this IHtmlHelper html, string operation, string type = null) where T : ACLRule, new()
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return currentUser != null &&
                   new T().CanAccess(currentUser, operation, type);
        }
    }

}