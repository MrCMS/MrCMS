using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities;
using MrCMS.Helpers;
using MrCMS.TextSearch.Services;
using MrCMS.Web.Admin.Models.Search;
using NHibernate;
using X.PagedList;

namespace MrCMS.Web.Admin.Services.Search
{
    public class AdminSearchService : IAdminSearchService
    {
        private readonly ITextSearcher _textSearcher;
        private readonly ISession _session;

        public AdminSearchService(ITextSearcher textSearcher, ISession session)
        {
            _textSearcher = textSearcher;
            _session = session;
        }

        public async Task<IPagedList<AdminSearchResult>> Search(ITextSearcher.PagedTextSearcherQuery model)
        {
            var results = await _textSearcher.SearchPaged(model);

            return new StaticPagedList<AdminSearchResult>(results.Select(x => new AdminSearchResult
                (x, _session.Get(x.SystemType, x.EntityId) as SystemEntity)), results);
        }

        public List<SelectListItem> GetTypeOptions()
        {
            return _textSearcher.GetTypes().OrderBy(x => x.Name)
                .BuildSelectItemList(x => x.Name.BreakUpString(), x => x.Name, emptyItemText: "Any");
        }
    }
}