using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Web.Apps.Core.Widgets;

namespace MrCMS.Web.Apps.Core.DbConfiguration
{
    public class PlainTextWidgetOveride : IAutoMappingOverride<PlainTextWidget>
    {
        public void Override(AutoMapping<PlainTextWidget> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}