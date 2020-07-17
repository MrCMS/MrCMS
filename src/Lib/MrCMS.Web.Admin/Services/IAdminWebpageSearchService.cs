using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IAdminWebpageSearchService
    {
        IPagedList<Webpage> Search(AdminWebpageSearchQuery model);
        IEnumerable<QuickSearchResult> QuickSearch(AdminWebpageSearchQuery model);
        IEnumerable<Document> GetBreadCrumb(int? parentId);
        List<SelectListItem> GetDocumentTypes(string type);
        List<SelectListItem> GetParentsList();
    }
}