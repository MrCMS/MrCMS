using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Messaging;

namespace MrCMS.DbConfiguration.Overrides
{
    public class MessageTemplateOverride : IAutoMappingOverride<MessageTemplate>
    {
        public void Override(AutoMapping<MessageTemplate> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("MessageTempateType");
            mapping.Map(template => template.Body).CustomType<VarcharMax>().Length(4001);
        }
    }
}