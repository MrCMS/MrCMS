using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageAdminSearchService
    {
        IPagedList<Webpage> Search(WebpageSearchQuery searchQuery);
    }
}