using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultFailedLoginAttemptMessageTemplate : GetDefaultTemplate<FailedLoginAttemptMessageTemplate>
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultFailedLoginAttemptMessageTemplate(IGetCurrentSite getCurrentSite, ISystemConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public override async Task<FailedLoginAttemptMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var site = await _getCurrentSite.GetSite();
            var fromAddress = !string.IsNullOrWhiteSpace(mailSettings.SystemEmailAddress)
                ? mailSettings.SystemEmailAddress
                : "test@example.com";
            return new FailedLoginAttemptMessageTemplate
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
            };
        }
    }
}