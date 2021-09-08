using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;
using WebPush;

namespace MrCMS.Website.PushNotifications
{
    public class GetWebPushSettings : IGetWebPushSettings
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private readonly ICurrentSiteLocator _siteLocator;

        public GetWebPushSettings(IConfigurationProvider configurationProvider,
            ISystemConfigurationProvider systemConfigurationProvider, ICurrentSiteLocator siteLocator)
        {
            _configurationProvider = configurationProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            _siteLocator = siteLocator;
        }

        public async Task<WebPushSettings> GetSettings()
        {
            var settings = _configurationProvider.GetSiteSettings<WebPushSettings>();

            if (string.IsNullOrWhiteSpace(settings.VapidPrivateKey))
            {
                var mailSettings = _systemConfigurationProvider.GetSystemSettings<MailSettings>();
                var keys = VapidHelper.GenerateVapidKeys();
                settings.VapidPrivateKey = keys.PrivateKey;
                settings.VapidPublicKey = keys.PublicKey;
                settings.VapidSubject = $"mailto:{mailSettings.SystemEmailAddress}";
                var site = _siteLocator.GetCurrentSite();
                settings.DefaultNotificationTitle = site.Name;
                await _configurationProvider.SaveSettings(settings);
            }

            return settings;
        }
    }
}