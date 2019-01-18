using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using WebPush;

namespace MrCMS.Website.PushNotifications
{
    public class GetWebPushSettings : IGetWebPushSettings
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISystemConfigurationProvider _systemConfigurationProvider;
        private readonly Site _site;

        public GetWebPushSettings(IConfigurationProvider configurationProvider, ISystemConfigurationProvider systemConfigurationProvider, Site site)
        {
            _configurationProvider = configurationProvider;
            _systemConfigurationProvider = systemConfigurationProvider;
            _site = site;
        }

        public WebPushSettings GetSettings()
        {
            var settings = _configurationProvider.GetSiteSettings<WebPushSettings>();

            if (string.IsNullOrWhiteSpace(settings.VapidPrivateKey))
            {
                var mailSettings = _systemConfigurationProvider.GetSystemSettings<MailSettings>();
                var keys = VapidHelper.GenerateVapidKeys();
                settings.VapidPrivateKey = keys.PrivateKey;
                settings.VapidPublicKey = keys.PublicKey;
                settings.VapidSubject = $"mailto:{mailSettings.SystemEmailAddress}";
                settings.DefaultNotificationTitle = _site.Name;
                _configurationProvider.SaveSettings(settings);
            }
            return settings;
        }
    }
}