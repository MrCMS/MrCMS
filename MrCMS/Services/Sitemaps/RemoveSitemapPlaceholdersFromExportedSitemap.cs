using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public class RemoveSitemapPlaceholdersFromExportedSitemap : SitemapGenerationInfo<SitemapPlaceholder>
    {
        public override bool ShouldAppend(SitemapPlaceholder webpage)
        {
            return false;
        }

        public override void Append(SitemapPlaceholder webpage, XElement urlset, XDocument xmlDocument)
        {
        }
    }
}