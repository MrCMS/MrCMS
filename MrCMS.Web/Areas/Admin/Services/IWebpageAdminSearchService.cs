using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Controllers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageAdminSearchService
    {
        IPagedList<Webpage> Search(WebpageSearchQuery searchQuery);
    }
}