using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultSend2FACodeMessageTemplate : GetDefaultTemplate<Send2FACodeMessageTemplate>
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultSend2FACodeMessageTemplate(IGetCurrentSite getCurrentSite, ISystemConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public override async Task<Send2FACodeMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var site = await _getCurrentSite.GetSite();
            var fromAddress = !string.IsNullOrWhiteSpace(mailSettings.SystemEmailAddress)
                ? mailSettings.SystemEmailAddress
                : "test@example.com";
            return new Send2FACodeMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - 2FA Code",
                Body =
                    "Your authorization code is: <strong>{TwoFactorCode}</strong>",
                IsHtml = true
            };
        }
    }
}