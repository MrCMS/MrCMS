using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities;
using MrCMS.Entities.Documents;

namespace MrCMS.DbConfiguration.Overrides
{
    public class DocumentOverride : IAutoMappingOverride<Document>
    {
        public void Override(AutoMapping<Document> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("DocumentType");
            mapping.HasMany(x => x.Children).KeyColumn("ParentId");
            mapping.HasManyToMany(document => document.Tags).Table("DocumentTags").Cascade.SaveUpdate();
            mapping.HasMany(document => document.Versions).KeyColumn("DocumentId").Cascade.All();
            mapping.IgnoreProperty(x=>x.UrlSegment);
            mapping.Map(x => x.DocumentType).Formula("DocumentType").Access.ReadOnly();
        }
    }
}