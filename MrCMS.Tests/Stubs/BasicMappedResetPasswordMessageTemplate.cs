using System;
using System.Collections.Generic;
using MrCMS.DbConfiguration.Mapping;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Tests.Stubs
{
    [MrCMSMapClass]
    public class BasicMappedResetPasswordMessageTemplate : MessageTemplate
    {
        public override MessageTemplate GetInitialTemplate()
        {
            return new BasicMappedResetPasswordMessageTemplate()
            {
                ToAddress = "{Email}",
            };
        }

        public override List<string> GetTokens()
        {
            return MessageTemplateProcessor.GetTokens<User>();
        }
    }
}