using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Website.Caching;
using NHibernate;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapService : ISitemapService
    {
        private const string SiteMapCacheKey = "sitemap.cache.key";
        public static readonly XNamespace RootNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace ImageNameSpace = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly ICacheManager _cacheManager;
        private readonly HttpRequestBase _request;
        private readonly ISession _session;
        private readonly ISitemapElementAppender _sitemapElementAppender;


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
            // cache this for 1 minute, in case it's under heavy use, as it's quite intensive
            return _cacheManager.Get(SiteMapCacheKey, () =>
            {
                IEnumerable<Webpage> allWebpages =
                    _session.QueryOver<Webpage>()
                        .Where(webpage => webpage.Published && webpage.IncludeInSitemap)
                        .Cacheable()
                        .List();

                var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
                var urlset = new XElement(RootNamespace + "urlset",
                    new XAttribute(XNamespace.Xmlns + "image", ImageNameSpace.NamespaceName)
                    );
                xmlDocument.Add(urlset);

                AppendChildren(allWebpages, urlset, xmlDocument);

                return xmlDocument.ToString();
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