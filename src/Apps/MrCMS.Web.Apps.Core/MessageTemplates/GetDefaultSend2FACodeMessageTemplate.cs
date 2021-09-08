using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultSend2FACodeMessageTemplate : GetDefaultTemplate<Send2FACodeMessageTemplate>
    {
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultSend2FACodeMessageTemplate(ICurrentSiteLocator siteLocator, MailSettings mailSettings)
        {
            _siteLocator = siteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<Send2FACodeMessageTemplate> Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            var site = _siteLocator.GetCurrentSite();
            return Task.FromResult(new Send2FACodeMessageTemplate
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
            });
        }
    }
}