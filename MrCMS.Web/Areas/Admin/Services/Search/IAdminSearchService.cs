using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services.Search
{
    public interface IAdminSearchService
    {
        IPagedList<AdminSearchResult> Search(AdminSearchQuery model);
    }
}