using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.DbConfiguration.Overrides
{
    public class LayoutAreaOverride : IAutoMappingOverride<LayoutArea>
    {
        public void Override(AutoMapping<LayoutArea> mapping)
        {
            mapping.HasMany(area => area.Widgets).OrderBy("DisplayOrder").Cascade.Delete();
        }
    }
}