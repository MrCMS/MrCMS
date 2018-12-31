using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services.Sitemaps
{
    public class DefaultSitemapGenerationInfo : ISitemapGenerationInfo
    {
        private readonly IGetLiveUrl _getLiveUrl;

        public DefaultSitemapGenerationInfo(IGetLiveUrl getLiveUrl)
        {
            _getLiveUrl = getLiveUrl;
        }
        public bool ShouldAppend(Webpage webpage)
        {
            return webpage.Published;
        }

        public void Append(Webpage webpage, XElement urlset, XDocument xmlDocument)
        {
            var publishOn = webpage.PublishOn;
            if (!publishOn.HasValue)
                return;
            var content = publishOn.Value.SitemapDateString();
            urlset.Add(new XElement(SitemapService.RootNamespace + "url",
                new XElement(SitemapService.RootNamespace + "loc", _getLiveUrl.GetAbsoluteUrl(webpage)),
                new XElement(SitemapService.RootNamespace + "lastmod", content)
                ));
        }
    }
}