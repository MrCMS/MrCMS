using Owin;

namespace MrCMS.Services
{
    public interface IStandardAuthConfigurationService
    {
        void ConfigureAuth(IAppBuilder app);
    }
}