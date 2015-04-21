using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IUniversalSearchIndexSearcher
    {
        List<UniversalSearchItemQuickSearch> QuickSearch(QuickSearchParams searchParams);
        IPagedList<AdminSearchResult> Search(AdminSearchQuery searchQuery);
    }
}