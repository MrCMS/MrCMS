using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.AddOns.Pages;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.DbConfiguration.Overrides
{
    public class GalleryOverride : IAutoMappingOverride<Gallery>
    {
        public void Override(AutoMapping<Gallery> mapping)
        {
            mapping.HasMany(gallery => gallery.Items).OrderBy("DisplayOrder").Cascade.Delete();
        }
    }
}