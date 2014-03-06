using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminPageStatsService
    {
        IList<WebpageStats> GetSummary();
    }

    public class AdminPageStatsService : IAdminPageStatsService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public AdminPageStatsService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public IList<WebpageStats> GetSummary()
        {
            WebpageStats countAlias = null;
            Webpage webpageAlias = null;
            return _session.QueryOver(() => webpageAlias)
                .Where(x => x.Site.Id == _site.Id)
                .SelectList(
                    builder =>
                        builder.SelectGroup(() => webpageAlias.DocumentType)
                            .WithAlias(() => countAlias.DocumentType)
                            .SelectCount(() => webpageAlias.Id)
                            .WithAlias(() => countAlias.NumberOfPages)
                            .SelectSubQuery(
                                QueryOver.Of<Webpage>()
                                    .Where(
                                        webpage =>
                                            webpage.Site.Id == _site.Id &&
                                            webpage.DocumentType == webpageAlias.DocumentType &&
                                            (webpage.PublishOn == null || webpage.PublishOn > CurrentRequestData.Now))
                                    .ToRowCountQuery())
                            .WithAlias(() => countAlias.NumberOfUnPublishedPages))
                .TransformUsing(Transformers.AliasToBean<WebpageStats>())
                .List<WebpageStats>();
        }
    }
}