using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents;

namespace MrCMS.DbConfiguration.Overrides
{
    public class DocumentOverride : IAutoMappingOverride<Document>
    {
        public void Override(AutoMapping<Document> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("DocumentType");
            mapping.HasMany(x => x.Children).KeyColumn("ParentId");
            mapping.HasManyToMany(document => document.Tags).Table("DocumentTags");
            mapping.HasMany(document => document.Versions).KeyColumn("DocumentId").Cascade.All();
        }
    }

    public class DocumentVersionOverride : IAutoMappingOverride<DocumentVersion>
    {
        public void Override(AutoMapping<DocumentVersion> mapping)
        {
            mapping.Map(version => version.Data).CustomType<VarcharMax>().Length(4001);
        }
    }
}