using System.Threading.Tasks;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultResetPasswordMessageTemplate : GetDefaultTemplate<ResetPasswordMessageTemplate>
    {
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultResetPasswordMessageTemplate(ICurrentSiteLocator currentSiteLocator, MailSettings mailSettings)
        {
            _currentSiteLocator = currentSiteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<ResetPasswordMessageTemplate> Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            var site = _currentSiteLocator.GetCurrentSite();
            return Task.FromResult(new ResetPasswordMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - Password Reset Request",
                Body = "To reset your password please click <a href=\"{ResetPasswordUrl}\">here</a>",
                IsHtml = true,
            });
        }
    }
}