using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapService : ISitemapService
    {
        public static readonly XNamespace RootNamespace = "https://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace ImageNameSpace = "https://www.google.com/schemas/sitemap-image/1.1";
        private readonly IEnumerable<ISitemapDataSource> _additionalSources;
        private readonly IGetSitemapPath _getSitemapPath;
        private readonly IGetHomePage _getHomePage;
        private readonly IRepository<Webpage> _repository;
        private readonly Site _site;
        private readonly ISitemapElementAppender _sitemapElementAppender;
        private readonly SiteSettings _siteSettings;


        public SitemapService(IRepository<Webpage> repository, Site site, ISitemapElementAppender sitemapElementAppender,
            SiteSettings siteSettings, IEnumerable<ISitemapDataSource> additionalSources, IGetSitemapPath getSitemapPath,
            IGetHomePage getHomePage)
        {
            _repository = repository;
            _site = site;
            _sitemapElementAppender = sitemapElementAppender;
            _siteSettings = siteSettings;
            _additionalSources = additionalSources;
            _getSitemapPath = getSitemapPath;
            _getHomePage = getHomePage;
        }

        public void WriteSitemap()
        {
            var sitemapPath = _getSitemapPath.GetAbsolutePath(_site);

            var queryOver = _repository.Readonly()
                .Where(x => x.Published)
                .Where(x => x.IncludeInSitemap);

            queryOver = GetTypesToRemove()
                .Aggregate(queryOver, (current, type) => current.Where(x => x.GetType() != type));

            var list = queryOver
                .OrderBy(x => x.PublishOn)
                .Select(page => new SitemapData
                {
                    PublishOn = page.PublishOn,
                    RequiresSSL = page.RequiresSSL,
                    Url = page.UrlSegment
                }).ToList();

            list.AddRange(_additionalSources.SelectMany(x => x.GetAdditionalData()));
            var homepage = _getHomePage.Get();
            list.ForEach(
                sitemapData =>
                    sitemapData.SetAbsoluteUrl(_siteSettings, _site, 
                        homepage.UrlSegment
                        ));
            var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var urlset = new XElement(RootNamespace + "urlset",
                new XAttribute(XNamespace.Xmlns + "image", ImageNameSpace.NamespaceName)
                );
            xmlDocument.Add(urlset);

            AppendChildren(list, urlset, xmlDocument);

            File.WriteAllText(sitemapPath, xmlDocument.ToString(SaveOptions.DisableFormatting), Encoding.UTF8);
        }

        private IEnumerable<Type> GetTypesToRemove()
        {
            yield return typeof(Redirect);
            yield return typeof(SitemapPlaceholder);
            foreach (var type in _additionalSources.SelectMany(dataSource => dataSource.TypesToRemove))
            {
                yield return type;
            }
        }

        private void AppendChildren(IList<SitemapData> allData, XElement urlset, XDocument xmlDocument)
        {
            foreach (var data in allData)
            {
                _sitemapElementAppender.AddSiteMapData(data, urlset, xmlDocument);
            }
        }
    }
}