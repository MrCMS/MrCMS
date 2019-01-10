using System;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Settings;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class GetDefaultResetPasswordMessageTemplate : GetDefaultTemplate<ResetPasswordMessageTemplate>
    {
        private readonly Site _site;
        private readonly MailSettings _mailSettings;

        public GetDefaultResetPasswordMessageTemplate(Site site, MailSettings mailSettings)
        {
            _site = site;
            _mailSettings = mailSettings;
        }

        public override ResetPasswordMessageTemplate Get()
        {
            var fromAddress = !string.IsNullOrWhiteSpace(_mailSettings.SystemEmailAddress)
                ? _mailSettings.SystemEmailAddress
                : "test@example.com";
            return new ResetPasswordMessageTemplate
            {
                FromAddress = fromAddress,
                FromName = _site.Name,
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Password Reset Request", _site.Name),
                Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
                IsHtml = true,
            };
        }
    }
}