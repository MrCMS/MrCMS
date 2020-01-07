using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Apps.Admin.Models;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class AdminPageStatsService : IAdminPageStatsService
    {
        private readonly IRepository<Webpage> _repository;

        public AdminPageStatsService(IRepository<Webpage> repository)
        {
            _repository = repository;
        }

        public IList<WebpageStats> GetSummary()
        {
            var allByType = _repository.Readonly().GroupBy(x => x.DocumentType)
                .Select(x => new { type = x.Key, count = x.Count() }).ToList();
            var publishedByType = _repository.Readonly().Where(x => x.Published).GroupBy(x => x.DocumentType)
                .Select(x => new { type = x.Key, count = x.Count() }).ToDictionary(x => x.type, x => x.count);

            return allByType.Select(x => new WebpageStats
            {
                DocumentType = x.type,
                NumberOfPages = x.count,
                NumberOfUnPublishedPages = publishedByType.ContainsKey(x.type) ? publishedByType[x.type] : 0
            }).ToList();
            //WebpageStats countAlias = null;
            //Webpage webpageAlias = null;
            //return _session.QueryOver(() => webpageAlias)
            //    .Where(x => x.Site.Id == _site.Id)
            //    .SelectList(
            //        builder =>
            //            builder.SelectGroup(() => webpageAlias.DocumentType)
            //                .WithAlias(() => countAlias.DocumentType)
            //                .SelectCount(() => webpageAlias.Id)
            //                .WithAlias(() => countAlias.NumberOfPages)
            //                .SelectSubQuery(
            //                    QueryOver.Of<Webpage>()
            //                        .Where(
            //                            webpage =>
            //                                webpage.Site.Id == _site.Id &&
            //                                webpage.DocumentType == webpageAlias.DocumentType &&
            //                               !webpage.Published && !webpage.IsDeleted)
            //                        .ToRowCountQuery())
            //                .WithAlias(() => countAlias.NumberOfUnPublishedPages))
            //    .TransformUsing(Transformers.AliasToBean<WebpageStats>())
            //    .Cacheable()
            //    .List<WebpageStats>();
        }
    }
}