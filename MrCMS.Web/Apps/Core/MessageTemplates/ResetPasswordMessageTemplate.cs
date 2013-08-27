using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    [FriendlyClassName("Reset Password Message Template")]
    public class ResetPasswordMessageTemplate : MessageTemplate, IMessageTemplate<User>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var resetPasswordPage = session.QueryOver<ResetPasswordPage>().SingleOrDefault();

            string resetUrl = resetPasswordPage != null
                                  ? resetPasswordPage.AbsoluteUrl + "?id={ResetPasswordGuid}"
                                  : string.Empty;

            var fromName = CurrentRequestData.CurrentSite.Name;
            return new ResetPasswordMessageTemplate
                       {
                           ToAddress = "{Email}",
                           ToName = "{Name}",
                           Bcc = String.Empty,
                           Cc = String.Empty,
                           Subject = String.Format("{0} - Password Reset Request", fromName),
                           Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", resetUrl),
                           IsHtml = true
                       };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<User>();
        }
    }
}