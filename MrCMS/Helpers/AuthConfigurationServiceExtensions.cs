using MrCMS.Services;
using MrCMS.Website;
using Owin;

namespace MrCMS.Helpers
{
    public static class AuthConfigurationServiceExtensions
    {
        public static void ConfigureAuth(this IAppBuilder app)
        {
            var authConfigurationService = MrCMSApplication.Get<IAuthConfigurationService>();
            authConfigurationService.ConfigureAuth(app);
        }
    }
}