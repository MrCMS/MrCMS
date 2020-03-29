using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        Task<List<UniversalSearchItemQuickSearch>> QuickSearch(QuickSearchParams searchParams);
        Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery searchQuery);
    }
}