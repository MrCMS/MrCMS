using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Areas.Admin.Models.Search;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IAdminWebpageSearchService
    {
        IPagedList<Webpage> Search(AdminWebpageSearchQuery model);
        IEnumerable<QuickSearchResults> QuickSearch(AdminWebpageSearchQuery model);
        IEnumerable<Document> GetBreadCrumb(int? parentId);
    }
}