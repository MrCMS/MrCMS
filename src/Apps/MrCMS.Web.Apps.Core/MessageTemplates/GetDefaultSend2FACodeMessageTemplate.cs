using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultSend2FACodeMessageTemplate : GetDefaultTemplate<Send2FACodeMessageTemplate>
    {
        private readonly MailSettings _mailSettings;
        private readonly Site _site;

        public GetDefaultSend2FACodeMessageTemplate(Site site, MailSettings mailSettings)
        {
            _site = site;
            _mailSettings = mailSettings;
        }

        public override Send2FACodeMessageTemplate Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            return new Send2FACodeMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = _site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{_site.Name} - 2FA Code",
                Body =
                    "Your authorization code is: <strong>{TwoFactorCode}</strong>",
                IsHtml = true
            };
        }
    }
}