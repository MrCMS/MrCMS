using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Widget;

namespace MrCMS.DbConfiguration.Overrides
{
    public class WidgetOverride : IAutoMappingOverride<Widget>
    {
        public void Override(AutoMapping<Widget> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("WidgetType");
        }
    }
}