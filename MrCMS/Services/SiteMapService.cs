using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services
{
    public class SiteMapService : ISiteMapService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public SiteMapService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public string GetSiteMap(UrlHelper urlHelper)
        {
            //var websiteTree = GetWebsiteTree();
            var allWebpages =
                _session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Site.Id == _site.Id)
                        .Cacheable()
                        .List()
                        .Where(webpage => webpage.Published);

            var xmlDocument = new XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            var urlset =
                xmlDocument.AppendChild(xmlDocument.CreateElement("urlset"));


            var standardNs = xmlDocument.CreateAttribute("xmlns");
            standardNs.Value = "http://www.google.com/schemas/sitemap/0.9";
            urlset.Attributes.Append(standardNs);
            var imageNs = xmlDocument.CreateAttribute("xmlns:image");
            imageNs.Value = "http://www.google.com/schemas/sitemap-image/1.1";
            urlset.Attributes.Append(imageNs);

            AppendChildren(allWebpages, urlset, xmlDocument, urlHelper);

            StringBuilder sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            xmlDocument.WriteTo(new XmlTextWriter(tw));

            var content = sb.ToString();
            return content;
        }

        private void AppendChildren(IEnumerable<Webpage> allWebpages, XmlNode urlset, XmlDocument xmlDocument, UrlHelper urlHelper)
        {
            foreach (var webpage in allWebpages)
            {
                if (webpage != null && webpage.Published)
                {
                    var url = urlset.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "url", ""));
                    var loc = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "loc", ""));
                    loc.InnerText = urlHelper.AbsoluteContent(webpage.LiveUrlSegment);
                    webpage.AddCustomSitemapData(urlHelper, url, xmlDocument);
                    var lastMod = url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "lastmod", ""));
                    lastMod.InnerText = webpage.UpdatedOn.ToString("yyyy-MM-dd");
                }
            }
        }

    }
}