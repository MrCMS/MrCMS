using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services.Search
{
    public interface IAdminSearchService
    {
        IPagedList<AdminSearchResult> Search(AdminSearchQuery model);
    }
}