using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public interface ISitemapGenerationInfo
    {
        bool ShouldAppend(Webpage webpage);
        void Append(Webpage webpage, XElement urlset, XDocument xmlDocument);
    }
    public interface ISiteMapGenerationInfo<in T> where T : Webpage
    {
        bool ShouldAppend(T webpage);
        void Append(T webpage, XElement urlset, XDocument xmlDocument);
    }
}