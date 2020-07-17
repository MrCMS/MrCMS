using System.Collections.Generic;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        List<UniversalSearchItemQuickSearch> QuickSearch(QuickSearchParams searchParams);
        IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery);
    }
}