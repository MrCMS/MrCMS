using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Profiling;
using System.Linq;

namespace MrCMS.Website
{
    public static class MiniProfilerAuth
    {
        public static bool IsUserAllowedToSeeMiniProfilerUI(HttpRequest request)
        {
            var userService = request.HttpContext.RequestServices.GetRequiredService<IUserLookup>();
            if (userService == null)
            {
                return false;
            }

            User currentUser = userService.GetCurrentUser(request.HttpContext);
            if (currentUser == null) return false;

            var userRoleManager = request.HttpContext.RequestServices.GetRequiredService<IUserRoleManager>();
            return userRoleManager.IsAdmin(currentUser).GetAwaiter().GetResult();
        }

        public static bool ShouldStartFor(HttpRequest request)
        {
            var serviceProvider = request.HttpContext.RequestServices;
            var settings = serviceProvider.GetRequiredService<SiteSettings>();
            var miniProfilerEnabled = settings != null && settings.MiniProfilerEnabled;
            if (!miniProfilerEnabled)
            {
                return false;
            }

            var typesForEnable = TypeHelper.GetAllConcreteTypesAssignableFrom<IReasonToEnableMiniProfiler>();
            var typesForDisable = TypeHelper.GetAllConcreteTypesAssignableFrom<IReasonToDisableMiniProfiler>();

            if (typesForEnable.Select(type => serviceProvider.GetService(type) as IReasonToEnableMiniProfiler).Any(reason => reason.ShouldEnableFor(request)))
            {
                return true;
            }

            var shouldStartFor = !typesForDisable.Select(type=> serviceProvider.GetService(type) as IReasonToDisableMiniProfiler).Any(reason => reason.ShouldDisableFor(request));
            return shouldStartFor;
        }
    }
}