using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class WebpageAdminSearchService : IWebpageAdminSearchService
    {
        private readonly ISession _session;

        public WebpageAdminSearchService(ISession session)
        {
            _session = session;
        }

        public async Task<IPagedList<Webpage>> Search(WebpageSearchQuery searchQuery)
        {
            var query = _session.QueryOver<Webpage>().Where(x => x.Parent.Id == searchQuery.ParentId);

            if (!string.IsNullOrWhiteSpace(searchQuery.Query))
            {
                query = query.Where(x => x.Name.IsInsensitiveLike(searchQuery.Query, MatchMode.Anywhere));
            }

            return await query.PagedAsync(searchQuery.Page, 50);
        }
    }
}