using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides;

public class ContentRowOverride : IAutoMappingOverride<ContentRow>
{
    public void Override(AutoMapping<ContentRow> mapping)
    {
        mapping.Map(x => x.Data).MakeVarCharMax();
    }
}