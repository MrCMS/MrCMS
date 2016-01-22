using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Messaging;

namespace MrCMS.DbConfiguration.Overrides
{
    public class QueuedMessageOverride : IAutoMappingOverride<QueuedMessage>
    {
        public void Override(AutoMapping<QueuedMessage> mapping)
        {
            mapping.Map(message => message.Body).CustomType<VarcharMax>().Length(4001);
            mapping.HasMany(message => message.QueuedMessageAttachments).Cascade.Delete().KeyColumn("QueuedMessageId");
        }
    }
}