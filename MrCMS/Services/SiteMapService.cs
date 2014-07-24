using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Services
{
    public class SiteMapService : ISiteMapService
    {
        private readonly ICustomSiteMapService _customSiteMapService;
        private readonly ISession _session;
        private readonly Site _site;

        public SiteMapService(ISession session, Site site, ICustomSiteMapService customSiteMapService)
        {
            _session = session;
            _site = site;
            _customSiteMapService = customSiteMapService;
        }

        public string GetSiteMap()
        {
            IEnumerable<Webpage> allWebpages =
                _session.QueryOver<Webpage>()
                    .Where(webpage => webpage.Site.Id == _site.Id)
                    .Cacheable()
                    .List()
                    .Where(webpage => webpage.Published);

            var xmlDocument = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            XmlNode urlset = xmlDocument.AppendChild(xmlDocument.CreateElement("urlset"));

            XmlAttribute standardNs = xmlDocument.CreateAttribute("xmlns");
            standardNs.Value = "http://www.google.com/schemas/sitemap/0.9";
            urlset.Attributes.Append(standardNs);
            XmlAttribute imageNs = xmlDocument.CreateAttribute("xmlns:image");
            imageNs.Value = "http://www.google.com/schemas/sitemap-image/1.1";
            urlset.Attributes.Append(imageNs);

            AppendChildren(allWebpages, urlset, xmlDocument);

            var sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            xmlDocument.WriteTo(new XmlTextWriter(tw));

            string content = sb.ToString();
            return content;
        }

        private void AppendChildren(IEnumerable<Webpage> allWebpages, XmlNode urlset, XmlDocument xmlDocument)
        {
            foreach (Webpage webpage in allWebpages)
            {
                if (webpage != null && webpage.Published)
                {
                    XmlNode url = urlset.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "url", ""));
                    XmlNode loc = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "loc", ""));
                    loc.InnerText = webpage.AbsoluteUrl;
                    _customSiteMapService.AddCustomSiteMapData(webpage, url, xmlDocument);

                    XmlNode lastMod = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "lastmod", ""));
                    lastMod.InnerText = webpage.UpdatedOn.ToString("yyyy-MM-dd");
                }
            }
        }
    }
}