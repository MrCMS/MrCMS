using System.Threading.Tasks;
using MrCMS.Settings;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class ThirdPartyAuthSettingsAdminService : IThirdPartyAuthSettingsAdminService
    {
        private readonly IConfigurationProvider _configurationProvider;

        public ThirdPartyAuthSettingsAdminService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public Task<ThirdPartyAuthSettings> GetSettingsAsync()
        {
            return Task.Run(() => GetSettings());
        }

        public ThirdPartyAuthSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<ThirdPartyAuthSettings>();
        }

        public Task SaveSettingsAsync(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            return Task.Run(() => SaveSettings(thirdPartyAuthSettings));
        }

        public void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            _configurationProvider.SaveSettings(thirdPartyAuthSettings);
        }
    }
}