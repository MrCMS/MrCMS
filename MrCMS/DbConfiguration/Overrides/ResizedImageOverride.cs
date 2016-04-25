using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.DbConfiguration.Overrides
{
    public class ResizedImageOverride : IAutoMappingOverride<ResizedImage>
    {
        public void Override(AutoMapping<ResizedImage> mapping)
        {
            mapping.Map(file => file.Url).Length(1000);
        }
    }
}