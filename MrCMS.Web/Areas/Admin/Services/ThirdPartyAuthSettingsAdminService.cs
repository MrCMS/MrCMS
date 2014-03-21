using System.Threading;
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

        public async Task<ThirdPartyAuthSettings> GetSettingsAsync()
        {
            return
                await
                Task.Factory.StartNew(() => GetSettings(), CancellationToken.None, TaskCreationOptions.None,
                                      TaskScheduler.FromCurrentSynchronizationContext());
        }

        public ThirdPartyAuthSettings GetSettings()
        {
            return _configurationProvider.GetSiteSettings<ThirdPartyAuthSettings>();
        }

        public async Task SaveSettingsAsync(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            await
                Task.Factory.StartNew(() => SaveSettings(thirdPartyAuthSettings), CancellationToken.None,
                                      TaskCreationOptions.None,
                                      TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SaveSettings(ThirdPartyAuthSettings thirdPartyAuthSettings)
        {
            _configurationProvider.SaveSettings(thirdPartyAuthSettings);
        }
    }
}