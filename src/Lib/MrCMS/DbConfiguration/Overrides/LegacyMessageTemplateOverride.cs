using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Messaging;

namespace MrCMS.DbConfiguration.Overrides
{
    public class LegacyMessageTemplateOverride : IAutoMappingOverride<LegacyMessageTemplate>
    {
        public void Override(AutoMapping<LegacyMessageTemplate> mapping)
        {
            mapping.Table("MessageTemplate");
        }
    }
}