using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        Task<List<UniversalSearchItemQuickSearch>> QuickSearch(QuickSearchParams searchParams);
        Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery searchQuery);
    }
}