using System.Threading.Tasks;
using System.Xml.Linq;
using MrCMS.Helpers;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapElementAppender : ISitemapElementAppender
    {
        public Task AddSiteMapData(SitemapData sitemapData, XElement urlset, XDocument xmlDocument)
        {
            if (sitemapData == null)
                return Task.CompletedTask;
            var publishOn = sitemapData.PublishOn;
            if (!publishOn.HasValue)
                return Task.CompletedTask;
            var content = publishOn.Value.SitemapDateString();
            urlset.Add(new XElement(SitemapService.RootNamespace + "url",
                new XElement(SitemapService.RootNamespace + "loc", sitemapData.AbsoluteUrl),
                new XElement(SitemapService.RootNamespace + "lastmod", content)
            ));
            return Task.CompletedTask;
        }
    }
}