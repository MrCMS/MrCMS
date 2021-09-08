using System.Threading.Tasks;
using System.Xml.Linq;

namespace MrCMS.Services.Sitemaps
{
    public interface ISitemapElementAppender
    {
        Task AddSiteMapData(SitemapData webpage, XElement urlset, XDocument xmlDocument);
    }
}