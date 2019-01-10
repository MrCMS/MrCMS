using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultSuccessfulLoginAttemptMessageTemplate : GetDefaultTemplate<SuccessfulLoginAttemptMessageTemplate>
    {
        private readonly MailSettings _mailSettings;
        private readonly Site _site;

        public GetDefaultSuccessfulLoginAttemptMessageTemplate(Site site, MailSettings mailSettings)
        {
            _site = site;
            _mailSettings = mailSettings;
        }

        public override SuccessfulLoginAttemptMessageTemplate Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            return new SuccessfulLoginAttemptMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = _site.Name,
                ToAddress = "{Email}",
                ToName = "",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{_site.Name} - Successful Login Attempt",
                Body =
                    "<p>There has been a successful login attempt on {SiteName}.<p>" +
                    "<p>IP: {IpAddress}</p>" +
                    "<p>User Agent: {UserAgent}</p>",
                IsHtml = true
            };
        }
    }
}