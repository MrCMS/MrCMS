using System;
using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ResetPasswordMessageTemplate : MessageTemplateBase<User> //MessageTemplate, IMessageTemplate<User>
    {
        public ResetPasswordMessageTemplate()
        {
            FromAddress = "test@example.com";
            FromName = CurrentRequestData.CurrentSite.Name;
            ToAddress = "{Email}";
            ToName = "{Name}";
            Bcc = String.Empty;
            Cc = String.Empty;
            Subject = String.Format("{0} - Password Reset Request", CurrentRequestData.CurrentSite.Name);
            Body = string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}");
            IsHtml = true;

        }
        //public override MessageTemplate GetInitialTemplate(ISession session)
        //{
        //    var fromName = ;
        //    return new ResetPasswordMessageTemplate
        //    {
        //    };
        //}

        //public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        //{
        //    return messageTemplateParser.GetAllTokens<User>();
        //}
    }
}