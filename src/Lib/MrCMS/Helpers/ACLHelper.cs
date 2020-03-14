using System.Threading.Tasks;
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
        public static async Task<bool> CanAccess<T>(this IHtmlHelper html, string operation) where T : ACLRule, new()
        {
            var currentUser = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IGetCurrentUser>().Get();
            return await html.CanAccess<T>(currentUser, operation);
        }

        public static async Task<bool> CanAccess<T>(this IHtmlHelper html, User user, string operation) where T : ACLRule, new()
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return user != null && await accessChecker.CanAccess<T>(operation, user);
        }
    }

}