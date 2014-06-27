using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Tests.Stubs
{
    public class BasicMessageTemplate : MessageTemplate
    {
        public override MessageTemplate GetInitialTemplate(ISession session)
        {
            return new BasicMessageTemplate
            {
                ToAddress = "{Email}",
            };
        }

        public override List<string> GetTokens(IMessageTemplateParser messageTemplateParser)
        {
            return new List<string>();
        }
    }
}