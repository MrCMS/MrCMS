using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Apps.Core.MessageTemplates
{
    public class ResetPasswordMessageTemplate : MessageTemplateBase<User>
    {
    }
}