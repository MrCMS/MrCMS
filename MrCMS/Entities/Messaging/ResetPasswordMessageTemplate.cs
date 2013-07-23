using System;
using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;
using MrCMS.Settings;
using MrCMS.Helpers;

namespace MrCMS.Entities.Messaging
{
    [FriendlyClassName("Reset Password Message Template")]
    public class ResetPasswordMessageTemplate : MessageTemplate, IMessageTemplate
    {
        public override MessageTemplate GetInitialTemplate()
        {
            var resetUrl = (CurrentRequestData.CurrentContext!=null && 
                CurrentRequestData.CurrentContext.Request!=null && 
                CurrentRequestData.CurrentContext.Request.Url!=null)?
                           CurrentRequestData.CurrentContext.Request.Url.Scheme + "://" +
                           CurrentRequestData.CurrentContext.Request.Url.Authority +
                           "Login/PasswordReset/{ResetPasswordGuid}":"#";
            return new ResetPasswordMessageTemplate()
                {
                    FromAddress = MrCMSApplication.Get<MailSettings>().SystemEmailAddress,
                    FromName = CurrentRequestData.CurrentSite.Name,
                    ToAddress = "{Email}",
                    ToName = "{Name}",
                    Bcc = String.Empty,
                    Cc = String.Empty,
                    Subject = String.Format("{0}Password Reset Request",CurrentRequestData.CurrentSite.Name+" - "),
                    Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", resetUrl),
                    IsHtml = true
                };
        }

        public virtual List<string> GetTokens()
        {
            return MessageTemplateProcessor.GetTokens<User>();
        }
    }
}