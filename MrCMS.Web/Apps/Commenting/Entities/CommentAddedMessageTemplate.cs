using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Apps.Commenting.Entities
{
    public class CommentAddedMessageTemplate : MessageTemplate, IMessageTemplate<Comment>
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            return new CommentAddedMessageTemplate
                   {
                       FromAddress = "test@example.com",
                       FromName = "Site Owner",
                       ToAddress = "{Email}",
                       ToName = "{Name}",
                       Bcc = String.Empty,
                       Cc = String.Empty,
                       Subject = "Comment Added - #{Id}",
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