using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.TextSearch.Services;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public class QuickSearcher : IQuickSearcher
    {
        private readonly ITextSearcher _textSearcher;

        public QuickSearcher(ITextSearcher textSearcher)
        {
            _textSearcher = textSearcher;
        }

        public async Task<List<QuickSearchResult>> QuickSearch(QuickSearchParams searchParams)
        {
            if (string.IsNullOrWhiteSpace(searchParams.Term))
                return new List<QuickSearchResult>();
            
            var items = await _textSearcher.Search(new ITextSearcher.TextSearcherQuery
            {
                Term = searchParams.Term,
                Type = searchParams.Type,
                ResultSize = 10
            });

            return items.Select(x => new QuickSearchResult
            {
                id = x.EntityId,
                value = x.DisplayName,
                systemType = x.SystemType,
                entityType = x.EntityType,
            }).ToList();
        }
    }
}