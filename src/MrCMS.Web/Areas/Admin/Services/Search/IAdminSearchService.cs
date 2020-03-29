using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services.Search
{
    public interface IAdminSearchService
    {
        Task<IPagedList<AdminSearchResult>> Search(AdminSearchQuery model);
    }
}