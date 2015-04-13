using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services.Search
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