using System;
using System.Threading.Tasks;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultResetPasswordMessageTemplate : GetDefaultTemplate<ResetPasswordMessageTemplate>
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GetDefaultResetPasswordMessageTemplate(IGetCurrentSite getCurrentSite, ISystemConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public override async Task<ResetPasswordMessageTemplate> Get()
        {
            var mailSettings = await _configurationProvider.GetSystemSettings<MailSettings>();
            var site = await _getCurrentSite.GetSite();
            var fromAddress = !string.IsNullOrWhiteSpace(mailSettings.SystemEmailAddress)
                ? mailSettings.SystemEmailAddress
                : "test@example.com";
            return new ResetPasswordMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Password Reset Request", site.Name),
                Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
                IsHtml = true,
            };
        }
    }
}