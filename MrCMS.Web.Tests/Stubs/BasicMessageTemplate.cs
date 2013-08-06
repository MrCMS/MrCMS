using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Web.Tests.Stubs
{
    [MrCMSMapClass]
    public class BasicMessageTemplate : MessageTemplate
    {
        public override MessageTemplate GetInitialTemplate()
        {
            return new BasicMessageTemplate();
        }

        public override List<string> GetTokens()
        {
            return MessageTemplateProcessor.GetTokens<User>();
        }
    }
}