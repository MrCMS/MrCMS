using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Paging;
using MrCMS.Web.Apps.Core.Models.Search;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public interface IWebpageSearchService
    {
        IPagedList<Webpage> Search(WebpageSearchQuery model);
        IEnumerable<Document> GetBreadCrumb(int? parentId);
    }
}
