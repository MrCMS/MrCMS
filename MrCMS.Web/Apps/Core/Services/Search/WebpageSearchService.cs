using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Indexing.Querying;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Indexing;
using MrCMS.Web.Apps.Core.Models.Search;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly ISearcher<Webpage, WebpageSearchIndexDefinition> _documentSearcher;
        private readonly IGetBreadcrumbs _getBreadcrumbs;

        public WebpageSearchService(ISearcher<Webpage, WebpageSearchIndexDefinition> documentSearcher, IGetBreadcrumbs getBreadcrumbs)
        {
            _documentSearcher = documentSearcher;
            _getBreadcrumbs = getBreadcrumbs;
        }

        public IPagedList<Webpage> Search(WebpageSearchQuery model)
        {
            return _documentSearcher.Search(model.GetQuery(), model.Page);
        }

        public IEnumerable<Document> GetBreadCrumb(int? parentId)
        {
            return _getBreadcrumbs.Get(parentId).Reverse();
        }
    }
}