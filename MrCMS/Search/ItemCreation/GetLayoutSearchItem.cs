using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetLayoutSearchItem : GetUniversalSearchItemBase<Layout>
    {
        private readonly IEnumerable<IGetLayoutSearchTerms> _getLayoutSearchTerms;

        public GetLayoutSearchItem(IEnumerable<IGetLayoutSearchTerms> getLayoutSearchTerms)
        {
            _getLayoutSearchTerms = getLayoutSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(Layout entity)
        {
            HashSet<string> searchTerms = _getLayoutSearchTerms.SelectMany(terms => terms.Get(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, searchTerms);
        }

        private UniversalSearchItem GetUniversalSearchItem(Layout entity, HashSet<string> searchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/layout/edit/" + entity.Id,
                Id = entity.Id,
                SearchTerms = searchTerms,
                SystemType = typeof (Layout).FullName
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Layout> entities)
        {
            var dictionary = new Dictionary<Layout, HashSet<string>>();
            foreach (IGetLayoutSearchTerms getLayoutSearchTerms in _getLayoutSearchTerms)
            {
                Dictionary<Layout, HashSet<string>> terms = getLayoutSearchTerms.Get(entities);
                foreach (Layout key in terms.Keys)
                {
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key].AddRange(terms[key]);
                    }
                    dictionary[key] = terms[key];
                }
            }

            return
                entities.Select(layout =>
                    GetUniversalSearchItem(layout,
                        dictionary.ContainsKey(layout) ? dictionary[layout] : new HashSet<string>())).ToHashSet();
        }
    }
}