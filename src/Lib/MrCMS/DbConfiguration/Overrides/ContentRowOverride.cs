using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides;

public class ContentRowOverride : IAutoMappingOverride<ContentBlock>
{
    public void Override(AutoMapping<ContentBlock> mapping)
    {
        mapping.Map(x => x.Data).MakeVarCharMax();
    }
}