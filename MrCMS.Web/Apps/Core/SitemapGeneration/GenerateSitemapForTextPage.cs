using System.Web.Mvc;
using System.Xml.Linq;
using MrCMS.Helpers;
using MrCMS.Services.Sitemaps;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.SitemapGeneration
{
    public class GenerateSitemapForTextPage : SitemapGenerationInfo<TextPage>
    {
        private readonly UrlHelper _urlHelper;

        public GenerateSitemapForTextPage(UrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public override bool ShouldAppend(TextPage webpage)
        {
            return webpage.Published;
        }

        public override void Append(TextPage webpage, XElement urlset, XDocument xmlDocument)
        {
            var publishOn = webpage.PublishOn;
            if (!publishOn.HasValue)
                return;
            var content = webpage.PublishOn.Value.SitemapDateString();
            var urlNode = new XElement(SitemapService.RootNamespace + "url",
                   new XElement(SitemapService.RootNamespace + "loc", webpage.AbsoluteUrl),
                   new XElement(SitemapService.RootNamespace + "lastmod", content)
                   );

            if (!string.IsNullOrWhiteSpace(webpage.FeatureImage))
                urlNode.Add(new XElement(SitemapService.ImageNameSpace + "image",
                    new XElement(SitemapService.ImageNameSpace + "loc", _urlHelper.AbsoluteContent(webpage.FeatureImage))));

            urlset.Add(urlNode);
        }
    }
}