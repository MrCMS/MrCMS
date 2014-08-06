using MrCMS.Settings;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class ThirdPartyAuthSettingsAdminService : IThirdPartyAuthSettingsAdminService
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public ThirdPartyAuthSettingsAdminService(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public ThirdPartyAuthSettings GetSettings()
        {
            return _configurationProvider.GetSystemSettings<ThirdPartyAuthSettings>();
        }

        public void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            _configurationProvider.SaveSettings(thirdPartyAuthSettings);
        }
    }
}