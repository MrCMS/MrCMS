using Owin;

namespace MrCMS.Services
{
    public interface IAuthConfigurationService
    {
        void ConfigureAuth(IAppBuilder app);
    }
}