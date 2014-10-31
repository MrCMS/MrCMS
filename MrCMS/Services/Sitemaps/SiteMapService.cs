using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapService : ISitemapService
    {
        private const string SiteMapCacheKey = "sitemap.cache.key";
        private readonly ICacheManager _cacheManager;
        private readonly HttpRequestBase _request;
        private readonly ISession _session;
        private readonly ISitemapElementAppender _sitemapElementAppender;
        public static readonly XNamespace RootNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace ImageNameSpace = "http://www.google.com/schemas/sitemap-image/1.1";


        public SitemapService(ISession session, ISitemapElementAppender sitemapElementAppender,
            ICacheManager cacheManager, HttpRequestBase request)
        {
            _session = session;
            _sitemapElementAppender = sitemapElementAppender;
            _cacheManager = cacheManager;
            _request = request;
        }

        public string GetSitemap()
        {
            // cache this for 1 minute, incase it's under heavy use, as it's quite intensive
            return _cacheManager.Get(SiteMapCacheKey, () =>
            {
                IEnumerable<Webpage> allWebpages =
                    _session.QueryOver<Webpage>()
                        .Cacheable()
                        .List()
                        .Where(webpage => webpage.Published);

                var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
                var urlset = new XElement(RootNamespace + "urlset",
                    new XAttribute(XNamespace.Xmlns + "image", ImageNameSpace.NamespaceName)
                    );
                xmlDocument.Add(urlset);

                AppendChildren(allWebpages, urlset, xmlDocument);

                return xmlDocument.ToString(); // content;
                //var xmlDocument = new XmlDocument();
                //XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
                //xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

                //XmlNode urlset = xmlDocument.AppendChild(xmlDocument.CreateElement("urlset"));

                //XmlAttribute standardNs = xmlDocument.CreateAttribute("xmlns");
                //standardNs.Value = "http://www.google.com/schemas/sitemap/0.9";
                //urlset.Attributes.Append(standardNs);
                //XmlAttribute imageNs = xmlDocument.CreateAttribute("xmlns:image");
                //imageNs.Value = "http://www.google.com/schemas/sitemap-image/1.1";
                //urlset.Attributes.Append(imageNs);

                //AppendChildren(allWebpages, urlset, xmlDocument);

                //var sb = new StringBuilder();
                //TextWriter tw = new StringWriter(sb);
                //xmlDocument.WriteTo(new XmlTextWriter(tw));

                //string content = sb.ToString();
                //return content;
            }, GetCacheLength(), CacheExpiryType.Absolute);
        }

        private TimeSpan GetCacheLength()
        {
            return _request.IsLocal
                ? TimeSpan.Zero
                : TimeSpan.FromMinutes(1);
        }

        private void AppendChildren(IEnumerable<Webpage> allWebpages, XElement urlset, XDocument xmlDocument)
        {
            foreach (Webpage webpage in allWebpages.Where(webpage => _sitemapElementAppender.ShouldAppend(webpage)))
            {
                _sitemapElementAppender.AddCustomSiteMapData(webpage, urlset, xmlDocument);
            }
        }
    }
}