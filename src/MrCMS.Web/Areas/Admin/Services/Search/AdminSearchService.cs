using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services.Search
{
    public class AdminSearchService : IAdminSearchService
    {
        private readonly IUniversalSearchIndexSearcher _universalSearchIndexSearcher;

        public AdminSearchService(IUniversalSearchIndexSearcher universalSearchIndexSearcher)
        {
            _universalSearchIndexSearcher = universalSearchIndexSearcher;
        }

        public async Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery model)
        {
            return await _universalSearchIndexSearcher.Search(model);
        }
    }
}