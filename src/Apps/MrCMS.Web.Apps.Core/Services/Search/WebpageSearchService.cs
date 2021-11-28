using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Website;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Helpers;
using NHibernate;
using X.PagedList;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Web.Apps.Core.Services.Search
{
    public class WebpageSearchService : IWebpageSearchService
    {
        private readonly IGetBreadcrumbs _getBreadcrumbs;
        private readonly IGetDateTimeNow _getDateTimeNow;
        private readonly ISession _session;

        public WebpageSearchService(
            IGetBreadcrumbs getBreadcrumbs,
            IGetDateTimeNow getDateTimeNow, ISession session)
        {
            _getBreadcrumbs = getBreadcrumbs;
            _getDateTimeNow = getDateTimeNow;
            _session = session;
        }

        public async Task<IPagedList<Webpage>> Search(WebpageSearchQuery model)
        {
            return await _session.Query<Webpage>().Where(x => x.Name.Contains(model.Term)).PagedAsync(model.Page, 20);
        }

        public async Task<IReadOnlyList<Document>> GetBreadCrumb(int? parentId)
        {
            return await _getBreadcrumbs.Get(parentId);
        }
    }
}