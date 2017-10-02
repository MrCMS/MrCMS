using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapService : ISitemapService
    {
        public static readonly XNamespace RootNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace ImageNameSpace = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly IEnumerable<ISitemapDataSource> _additionalSources;
        private readonly IGetSitemapPath _getSitemapPath;
        private readonly IStatelessSession _session;
        private readonly Site _site;
        private readonly ISitemapElementAppender _sitemapElementAppender;
        private readonly SiteSettings _siteSettings;


        public SitemapService(IStatelessSession session, Site site, ISitemapElementAppender sitemapElementAppender,
            SiteSettings siteSettings, IEnumerable<ISitemapDataSource> additionalSources, IGetSitemapPath getSitemapPath)
        {
            _session = session;
            _site = site;
            _sitemapElementAppender = sitemapElementAppender;
            _siteSettings = siteSettings;
            _additionalSources = additionalSources;
            _getSitemapPath = getSitemapPath;
        }

        public void WriteSitemap()
        {
            var sitemapPath = _getSitemapPath.GetPath(_site);

            SitemapData data = null;
            var queryOver = _session.QueryOver<Webpage>()
                .Where(x => !x.IsDeleted)
                .And(x => x.Site.Id == _site.Id)
                .And(x => x.Published)
                .And(x => x.IncludeInSitemap);

            queryOver = GetTypesToRemove()
                .Aggregate(queryOver, (current, type) => current.And(x => x.GetType() != type));

            var list = queryOver
                .OrderBy(x => x.PublishOn).Desc
                .SelectList(builder =>
                {
                    builder.Select(x => x.PublishOn).WithAlias(() => data.PublishOn);
                    builder.Select(x => x.UrlSegment).WithAlias(() => data.Url);
                    builder.Select(x => x.RequiresSSL).WithAlias(() => data.RequiresSSL);
                    return builder;
                }).TransformUsing(Transformers.AliasToBean<SitemapData>())
                .Cacheable()
                .List<SitemapData>().ToList();

            list.AddRange(_additionalSources.SelectMany(x => x.GetAdditionalData()));

            list.ForEach(
                sitemapData =>
                    sitemapData.SetAbsoluteUrl(_siteSettings, _site, CurrentRequestData.HomePage.UrlSegment));
            var xmlDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var urlset = new XElement(RootNamespace + "urlset",
                new XAttribute(XNamespace.Xmlns + "image", ImageNameSpace.NamespaceName)
                );
            xmlDocument.Add(urlset);

            AppendChildren(list, urlset, xmlDocument);

            File.WriteAllText(sitemapPath, xmlDocument.ToString(SaveOptions.DisableFormatting));
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