using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IAdminWebpageSearchService
    {
        Task<IPagedList<Webpage>> Search(AdminWebpageSearchQuery model);
        Task<IEnumerable<QuickSearchResult>> QuickSearch(AdminWebpageSearchQuery model);
        IEnumerable<Document> GetBreadCrumb(int? parentId);
        List<SelectListItem> GetDocumentTypes(string type);
        List<SelectListItem> GetParentsList();
    }
}