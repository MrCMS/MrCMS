using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageAdminSearchService
    {
        IPagedList<Webpage> Search(WebpageSearchQuery searchQuery);
    }
}