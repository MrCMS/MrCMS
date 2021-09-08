using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultConfirmEmailMessageTemplate : GetDefaultTemplate<ConfirmEmailMessageTemplate>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultConfirmEmailMessageTemplate(ICurrentSiteLocator currentSiteLocator, MailSettings mailSettings)
        {
            _currentSiteLocator = currentSiteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<ConfirmEmailMessageTemplate> Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            var site = _currentSiteLocator.GetCurrentSite();
            return Task.FromResult(new ConfirmEmailMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - Confirm Email",
                Body = "To confirm your email, please click <a href=\"{ConfirmEmailUrl}\">here</a>",
                IsHtml = true,
            });
        }
    }
}