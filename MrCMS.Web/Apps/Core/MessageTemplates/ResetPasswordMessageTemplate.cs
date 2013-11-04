using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ResetPasswordMessageTemplate : MessageTemplate, IMessageTemplate<User>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            var fromName = CurrentRequestData.CurrentSite.Name;
            return new ResetPasswordMessageTemplate
            {
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Password Reset Request", fromName),
                Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
                IsHtml = true
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<User>();
        }
    }
}