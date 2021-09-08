using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultFailedLoginAttemptMessageTemplate : GetDefaultTemplate<FailedLoginAttemptMessageTemplate>
    {
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly MailSettings _mailSettings;

        public GetDefaultFailedLoginAttemptMessageTemplate(ICurrentSiteLocator siteLocator, MailSettings mailSettings)
        {
            _siteLocator = siteLocator;
            _mailSettings = mailSettings;
        }

        public override Task<FailedLoginAttemptMessageTemplate> Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            var site = _siteLocator.GetCurrentSite();
            return Task.FromResult(new FailedLoginAttemptMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - Failed Login Attempt",
                Body =
                    "<p>There has been a failed login attempt on {SiteName}.<p>" +
                    "<p>IP: {IpAddress}</p>" +
                    "<p>User Agent: {UserAgent}</p>",
                IsHtml = true
            });
        }
    }
}