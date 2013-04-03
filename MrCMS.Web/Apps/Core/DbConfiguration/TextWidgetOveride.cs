using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.DbConfiguration
{
    public class TextWidgetOveride : IAutoMappingOverride<TextWidget>
    {
        public void Override(AutoMapping<TextWidget> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}