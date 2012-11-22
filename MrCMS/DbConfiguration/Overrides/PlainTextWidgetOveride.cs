using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Widget;
using NHibernate.SqlTypes;

namespace MrCMS.DbConfiguration.Overrides
{
    public class PlainTextWidgetOveride : IAutoMappingOverride<PlainTextWidget>
    {
        public void Override(AutoMapping<PlainTextWidget> mapping)
        {
            mapping.Map(x => x.Text).CustomType<VarcharMax>().Length(4001);
        }
    }
}