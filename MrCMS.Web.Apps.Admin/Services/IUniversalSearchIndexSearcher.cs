using System.Collections.Generic;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        List<UniversalSearchItemQuickSearch> QuickSearch(QuickSearchParams searchParams);
        IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery);
    }
}