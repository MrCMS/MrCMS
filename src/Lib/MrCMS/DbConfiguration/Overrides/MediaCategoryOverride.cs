using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.DbConfiguration.Overrides
{
    public class MediaCategoryOverride : IAutoMappingOverride<MediaCategory>
    {
        public void Override(AutoMapping<MediaCategory> mapping)
        {
            mapping.HasMany(x => x.Files).KeyColumn("MediaCategoryId").Cascade.Delete();
        }
    }
}