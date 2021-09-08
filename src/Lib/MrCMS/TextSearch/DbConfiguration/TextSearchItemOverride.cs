using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.TextSearch.Entities;

namespace MrCMS.TextSearch.DbConfiguration
{
    public class TextSearchItemOverride : IAutoMappingOverride<TextSearchItem>
    {
        public void Override(AutoMapping<TextSearchItem> mapping)
        {
            mapping.Map(x => x.SystemType).UniqueKey("IQ_TextSearch");
            mapping.Map(x => x.EntityType).UniqueKey("IQ_TextSearch");
            mapping.Map(x => x.EntityId).UniqueKey("IQ_TextSearch");
            mapping.Map(x => x.SystemType).UniqueKey("IQ_TextSearch");
            // todo - add indexes
            // mapping.Map(x => x.Text);
        }
    }
}