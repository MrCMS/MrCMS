using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public interface ISitemapElementAppender
    {
        void AddCustomSiteMapData(Webpage webpage, XElement urlset, XDocument xmlDocument);
        bool ShouldAppend(Webpage webpage);
    }
}