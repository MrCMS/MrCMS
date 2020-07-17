using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services.Search
{
    public class AdminSearchService : IAdminSearchService
    {
        private readonly IUniversalSearchIndexSearcher _universalSearchIndexSearcher;

        public AdminSearchService(IUniversalSearchIndexSearcher universalSearchIndexSearcher)
        {
            _universalSearchIndexSearcher = universalSearchIndexSearcher;
        }

        public IPagedList<AdminSearchResult> Search(AdminSearchQuery model)
        {
            return _universalSearchIndexSearcher.Search(model);
        }
    }
}