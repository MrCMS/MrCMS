using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Services.Sitemaps
{
    public class DefaultSitemapGenerationInfo : ISitemapGenerationInfo
    {
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
                new XElement(SitemapService.RootNamespace + "loc", webpage.AbsoluteUrl),
                new XElement(SitemapService.RootNamespace + "lastmod", content)
                ));
            //XmlNode url = urlset.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "url", ""));
            //XmlNode loc = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "loc", ""));
            //loc.InnerText = webpage.AbsoluteUrl;
            //XmlNode lastMod = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "lastmod", ""));
            //lastMod.InnerText = webpage.UpdatedOn.ToString("yyyy-MM-dd");
        }
    }
}