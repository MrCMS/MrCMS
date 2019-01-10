using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents;

namespace MrCMS.DbConfiguration.Overrides
{
    public class DocumentVersionOverride : IAutoMappingOverride<DocumentVersion>
    {
        public void Override(AutoMapping<DocumentVersion> mapping)
        {
            mapping.Map(version => version.Data).CustomType<VarcharMax>().Length(4001);
        }
    }
}