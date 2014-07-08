using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration.Types;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.DbConfiguration.Overrides
{
    public class MediaFileOverride : IAutoMappingOverride<MediaFile>
    {
        public void Override(AutoMapping<MediaFile> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("MediaFileType");
            mapping.Map(file => file.Description).CustomType<VarcharMax>().Length(4001);
            mapping.Map(file => file.Title).Length(4000);
            mapping.Map(file => file.FileUrl).Length(1000);
            mapping.Map(file => file.FileUrl).Index("IX_MediaFile_FileUrl");
        }
    }
    public class ResizedImageOverride : IAutoMappingOverride<ResizedImage>
    {
        public void Override(AutoMapping<ResizedImage> mapping)
        {
            mapping.Map(file => file.Url).Length(1000);
        }
    }
}