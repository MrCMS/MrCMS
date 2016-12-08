using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public interface ISitemapElementAppender
    {
        void AddSiteMapData(SitemapData webpage, XElement urlset, XDocument xmlDocument);
    }
}