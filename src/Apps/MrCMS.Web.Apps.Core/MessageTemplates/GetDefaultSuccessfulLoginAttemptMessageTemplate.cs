using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultSuccessfulLoginAttemptMessageTemplate : GetDefaultTemplate<SuccessfulLoginAttemptMessageTemplate>
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultSuccessfulLoginAttemptMessageTemplate(IGetCurrentSite getCurrentSite, ISystemConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public override async Task<SuccessfulLoginAttemptMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var site = await _getCurrentSite.GetSite();
            var fromAddress = !string.IsNullOrWhiteSpace(mailSettings.SystemEmailAddress)
                ? mailSettings.SystemEmailAddress
                : "test@example.com";
            return new SuccessfulLoginAttemptMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "",
                Bcc = string.Empty,
                Cc = string.Empty,
                Subject = $"{site.Name} - Successful Login Attempt",
                Body =
                    "<p>There has been a successful login attempt on {SiteName}.<p>" +
                    "<p>IP: {IpAddress}</p>" +
                    "<p>User Agent: {UserAgent}</p>",
                IsHtml = true
            };
        }
    }
}