using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class PageWidgetSortOverride : IAutoMappingOverride<PageWidgetSort>
    {
        public void Override(AutoMapping<PageWidgetSort> mapping)
        {
            mapping.References(sort => sort.LayoutArea).Not.Nullable();
            mapping.References(sort => sort.Webpage).Not.Nullable();
            mapping.References(sort => sort.Widget).Not.Nullable();
        }
    }
}