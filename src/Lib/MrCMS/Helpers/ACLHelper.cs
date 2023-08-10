using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.ACL;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers
{
    public static class ACLHelper
    {
        public static async Task<bool> CanAccess<T>(this IHtmlHelper html, string operation) where T : ACLRule, new()
        {
            var currentUser = await html.ViewContext.HttpContext.RequestServices
                .GetRequiredService<IGetCurrentClaimsPrincipal>()
                .GetPrincipal();
            return await html.CanAccess<T>(currentUser, operation);
        }

        public static async Task<bool> CanAccess<T>(this IHtmlHelper html, ClaimsPrincipal user, string operation)
            where T : ACLRule, new()
        {
            var accessChecker = html.ViewContext.HttpContext.RequestServices.GetRequiredService<IAccessChecker>();
            return user != null && await accessChecker.CanAccess<T>(operation, user);
        }
    }
}