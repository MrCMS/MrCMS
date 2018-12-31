using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public abstract class SitemapGenerationInfo<T> : ISiteMapGenerationInfo<T>, ISitemapGenerationInfo where T : Webpage
    {
        bool ISitemapGenerationInfo.ShouldAppend(Webpage webpage)
        {
            var cast = webpage as T;
            if (cast == null)
                return false;
            return ShouldAppend(cast);
        }

        void ISitemapGenerationInfo.Append(Webpage webpage, XElement urlset, XDocument xmlDocument)
        {
            var cast = webpage as T;
            if (cast == null)
                return;
            Append(cast, urlset, xmlDocument);
        }

        public abstract bool ShouldAppend(T webpage);
        public abstract void Append(T webpage, XElement urlset, XDocument xmlDocument);
    }
}