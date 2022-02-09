using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultChangeEmailMessageTemplate : GetDefaultTemplate<ConfirmEmailChangeMessageTemplate>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultChangeEmailMessageTemplate(ICurrentSiteLocator currentSiteLocator, MailSettings mailSettings)
        {
            _currentSiteLocator = currentSiteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<ConfirmEmailChangeMessageTemplate> Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            var site = _currentSiteLocator.GetCurrentSite();
            return Task.FromResult(new ConfirmEmailChangeMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{NewEmail}",
                ToName = "{Name}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - Confirm Email Change",
                Body = "To confirm your email change, please click <a href=\"{ConfirmEmailChangeUrl}\">here</a>",
                IsHtml = true,
            });
        }
    }
}