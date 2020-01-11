using System.Threading.Tasks;
using MrCMS.Web.Apps.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services.Search
{
    public interface IAdminSearchService
    {
        Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery model);
    }
}