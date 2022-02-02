using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents;

namespace MrCMS.DbConfiguration.Overrides
{
    public class WebpageVersionOverride : IAutoMappingOverride<WebpageVersion>
    {
        public void Override(AutoMapping<WebpageVersion> mapping)
        {
            mapping.Map(version => version.Data).MakeVarCharMax();
        }
    }
}