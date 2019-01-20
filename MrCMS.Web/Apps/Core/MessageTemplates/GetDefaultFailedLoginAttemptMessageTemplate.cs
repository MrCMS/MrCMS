using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultFailedLoginAttemptMessageTemplate : GetDefaultTemplate<FailedLoginAttemptMessageTemplate>
    {
        private readonly MailSettings _mailSettings;
        private readonly Site _site;

        public GetDefaultFailedLoginAttemptMessageTemplate(Site site, MailSettings mailSettings)
        {
            _site = site;
            _mailSettings = mailSettings;
        }

        public override FailedLoginAttemptMessageTemplate Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            return new FailedLoginAttemptMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = _site.Name,
                ToAddress = "{Email}",
                ToName = "",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{_site.Name} - Failed Login Attempt",
                Body =
                    "<p>There has been a failed login attempt on {SiteName}.<p>" +
                    "<p>IP: {IpAddress}</p>" +
                    "<p>User Agent: {UserAgent}</p>",
                IsHtml = true
            };
        }
    }
}