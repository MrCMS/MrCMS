using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.DbConfiguration.Overrides
{
    public class LayoutOverride : IAutoMappingOverride<Layout>
    {
        public void Override(AutoMapping<Layout> mapping)
        {
            mapping.HasMany(x => x.LayoutAreas).KeyColumn("LayoutId").Cascade.All().Cache.ReadWrite();
            mapping.HasMany(layout => layout.Webpages).KeyColumn("LayoutId").Cascade.None();
        }
    }
}