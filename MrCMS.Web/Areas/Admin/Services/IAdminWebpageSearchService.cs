using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services
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