using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Entities
{
    public class CommentReportedMessageTemplate : MessageTemplate, IMessageTemplate<Comment>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            return new CommentReportedMessageTemplate
                   {
                       FromAddress = "test@example.com",
                       FromName = "Site Owner",
                       ToAddress = "{Email}",
                       ToName = "{Name}",
                       Bcc = String.Empty,
                       Cc = String.Empty,
                       Subject = "Comment Reported - #{Id}",
                       Body = "",
                       IsHtml = true
                   };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return messageTemplateParser.GetAllTokens<Comment>();
        }
    }
}