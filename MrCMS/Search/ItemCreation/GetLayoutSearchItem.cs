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
            HashSet<string> primarySearchTerms = _getLayoutSearchTerms.SelectMany(terms => terms.GetPrimary(entity)).ToHashSet();
            HashSet<string> secondarySearchTerms = _getLayoutSearchTerms.SelectMany(terms => terms.GetSecondary(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, primarySearchTerms, secondarySearchTerms);
        }

        private UniversalSearchItem GetUniversalSearchItem(Layout entity, HashSet<string> primarySearchTerms, HashSet<string> secondarySearchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/layout/edit/" + entity.Id,
                Id = entity.Id,
                PrimarySearchTerms = primarySearchTerms,
                SecondarySearchTerms = secondarySearchTerms,
                SystemType = typeof (Layout).FullName,
                CreatedOn = entity.CreatedOn,
                SearchGuid = entity.Guid
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Layout> entities)
        {
            var primaryDictionary = new Dictionary<Layout, HashSet<string>>();
            var secondaryDictionary = new Dictionary<Layout, HashSet<string>>();
            foreach (IGetLayoutSearchTerms getLayoutSearchTerms in _getLayoutSearchTerms)
            {
                Dictionary<Layout, HashSet<string>> primaryTerms = getLayoutSearchTerms.GetPrimary(entities);
                foreach (Layout key in primaryTerms.Keys)
                {
                    if (primaryDictionary.ContainsKey(key))
                    {
                        primaryDictionary[key].AddRange(primaryTerms[key]);
                    }
                    primaryDictionary[key] = primaryTerms[key];
                }

                Dictionary<Layout, HashSet<string>> secondaryTerms = getLayoutSearchTerms.GetSecondary(entities);
                foreach (Layout key in secondaryTerms.Keys)
                {
                    if (secondaryDictionary.ContainsKey(key))
                    {
                        secondaryDictionary[key].AddRange(secondaryTerms[key]);
                    }
                    secondaryDictionary[key] = secondaryTerms[key];
                }
            }

            return
                entities.Select(layout =>
                    GetUniversalSearchItem(layout,
                        primaryDictionary.ContainsKey(layout) ? primaryDictionary[layout] : new HashSet<string>(),
                        secondaryDictionary.ContainsKey(layout) ? secondaryDictionary[layout] : new HashSet<string>()
                        )).ToHashSet();
        }
    }
}