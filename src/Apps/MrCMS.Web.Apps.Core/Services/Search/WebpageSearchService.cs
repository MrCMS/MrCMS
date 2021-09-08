using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Website;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly IGetBreadcrumbs _getBreadcrumbs;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public WebpageSearchService(
            IGetBreadcrumbs getBreadcrumbs,
            IGetDateTimeNow getDateTimeNow)
        {
            _getBreadcrumbs = getBreadcrumbs;
            _getDateTimeNow = getDateTimeNow;
        }

        public Task<IPagedList<Webpage>> Search(WebpageSearchQuery model)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Document>> GetBreadCrumb(int? parentId)
        {
            return await _getBreadcrumbs.Get(parentId);
        }
    }
}