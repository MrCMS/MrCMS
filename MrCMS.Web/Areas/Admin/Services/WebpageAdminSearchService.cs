using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class WebpageAdminSearchService : IWebpageAdminSearchService
    {
        private readonly ISession _session;

        public WebpageAdminSearchService(ISession session)
        {
            _session = session;
        }

        public IPagedList<Webpage> Search(WebpageSearchQuery searchQuery)
        {
            var query = _session.QueryOver<Webpage>().Where(x => x.Parent.Id == searchQuery.ParentId);

            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                query = query.Where(x => x.Name.IsInsensitiveLike(searchQuery.Query, MatchMode.Anywhere));
            }

            return query.Paged(searchQuery.Page);
        }
    }
}