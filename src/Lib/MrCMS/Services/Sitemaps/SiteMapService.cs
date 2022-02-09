using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace MrCMS.Services.Sitemaps
{
    public class SitemapService : ISitemapService
    {
        public static readonly XNamespace RootNamespace = "http://www.sitemaps.org/schemas/sitemap/0.9";
        public static readonly XNamespace ImageNameSpace = "http://www.google.com/schemas/sitemap-image/1.1";
        private readonly IEnumerable<ISitemapDataSource> _additionalSources;
        private readonly IEnumerable<ISitemapPagesToRemove> _dataPagesToRemoves;
        private readonly IGetSitemapPath _getSitemapPath;
        private readonly IGetHomePage _getHomePage;
        private readonly IStatelessSession _session;
        private readonly ISitemapElementAppender _sitemapElementAppender;


        public SitemapService(IStatelessSession session, ISitemapElementAppender sitemapElementAppender,
            IEnumerable<ISitemapDataSource> additionalSources,
            IEnumerable<ISitemapPagesToRemove> dataPagesToRemoves,
            IGetSitemapPath getSitemapPath,
            IGetHomePage getHomePage)
        {
            _session = session;
            _sitemapElementAppender = sitemapElementAppender;
            _additionalSources = additionalSources;
            _dataPagesToRemoves = dataPagesToRemoves;
            _getSitemapPath = getSitemapPath;
            _getHomePage = getHomePage;
        }

        public async Task WriteSitemap()
        {
            var sites = await _session.Query<Site>().ToListAsync();
            foreach (var site in sites)
            {
                var sitemapPath = _getSitemapPath.GetAbsolutePath(site);

                SitemapData data = null;
                var queryOver = _session.QueryOver<Webpage>()
                    .Where(x => !x.IsDeleted)
                    .And(x => x.Site.Id == site.Id)
                    .And(x => x.Published)
                    .And(x => x.IncludeInSitemap);

                queryOver = GetTypesToRemove()
                    .Aggregate(queryOver, (current, type) => current.And(x => x.GetType() != type));


                var list = (await queryOver
                    .OrderBy(x => x.PublishOn).Desc
                    .SelectList(builder =>
                    {
                        builder.Select(x => x.Id).WithAlias(() => data.WebpageId);
                        builder.Select(x => x.PublishOn).WithAlias(() => data.PublishOn);
                        builder.Select(x => x.UrlSegment).WithAlias(() => data.Url);
                        return builder;
                    }).TransformUsing(Transformers.AliasToBean<SitemapData>())
                    .Cacheable()
                    .ListAsync<SitemapData>()).ToList();

                //removing after database call as there could be more than 2000 items to remove, in which case SQL server would fall over due to max params.
                var pageIdsToRemove = _dataPagesToRemoves.SelectMany(x => x.WebpageIds).ToHashSet();
                list.RemoveAll(x => pageIdsToRemove.Contains(x.WebpageId));

                list.AddRange(_additionalSources.SelectMany(x => x.GetAdditionalData()));
                var homepage = await _getHomePage.GetForSite(site);
                list.ForEach(
                    sitemapData =>
                    {
                        sitemapData.RequiresSSL = true;
                        sitemapData.SetAbsoluteUrl(site,
                            homepage.UrlSegment);
                    });

                var chunks = list.Chunk(SitemapChunkSize).ToList();
                var urls = new List<string>();
                for (var i = 0; i < chunks.Count; i++)
                {
                    var items = chunks[i];
                    var partDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
                    var urlset = new XElement(RootNamespace + "urlset",
                        new XAttribute(XNamespace.Xmlns + "image", ImageNameSpace.NamespaceName)
                    );
                    partDocument.Add(urlset);

                    await AppendChildren(items.ToList(), urlset, partDocument);

                    var path = _getSitemapPath.GetAbsolutePathForPart(site, i + 1);

                    urls.Add($"https://{site.BaseUrl.TrimEnd('/')}/{Path.GetFileName(path)}");

                    await using var fileStream = File.Open(path, FileMode.Create, FileAccess.Write);
                    await using var stream = new GZipStream(fileStream, CompressionLevel.Optimal);
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(partDocument.ToString(SaveOptions.DisableFormatting)));
                }

                var indexDocument = new XDocument(new XDeclaration("1.0", "utf-8", null));
                var sitemapIndex = new XElement(RootNamespace + "sitemapindex");
                indexDocument.Add(sitemapIndex);
                foreach (var url in urls)
                {
                    sitemapIndex.Add(new XElement(RootNamespace + "sitemap",
                        new XElement(RootNamespace + "loc", url),
                        new XElement(RootNamespace + "lastmod", DateTime.UtcNow.SitemapDateString())
                    ));
                }

                await File.WriteAllTextAsync(sitemapPath, indexDocument.ToString(SaveOptions.DisableFormatting),
                    Encoding.UTF8);
            }
        }

        public const int SitemapChunkSize = 40000;

        private IEnumerable<Type> GetTypesToRemove()
        {
            yield return typeof(Redirect);
            yield return typeof(SitemapPlaceholder);
            foreach (var type in _additionalSources.SelectMany(dataSource => dataSource.TypesToRemove))
            {
                yield return type;
            }
        }

        private async Task AppendChildren(IList<SitemapData> allData, XElement urlset, XDocument xmlDocument)
        {
            foreach (var data in allData)
            {
                await _sitemapElementAppender.AddSiteMapData(data, urlset, xmlDocument);
            }
        }
    }
}