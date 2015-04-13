using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetWebpageSearchItem : GetUniversalSearchItemBase<Webpage>
    {
        private readonly IEnumerable<IGetWebpageSearchTerms> _getWebpageSearchTerms;

        public GetWebpageSearchItem(IEnumerable<IGetWebpageSearchTerms> getWebpageSearchTerms)
        {
            _getWebpageSearchTerms = getWebpageSearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(Webpage entity)
        {
            HashSet<string> primarySearchTerms =
                _getWebpageSearchTerms.SelectMany(terms => terms.GetPrimary(entity)).ToHashSet();
            HashSet<string> secondarySearchTerms =
                _getWebpageSearchTerms.SelectMany(terms => terms.GetSecondary(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, primarySearchTerms, secondarySearchTerms);
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Webpage> entities)
        {
            var primaryDictionary = new Dictionary<Webpage, HashSet<string>>();
            foreach (IGetWebpageSearchTerms webpageSearchTerms in _getWebpageSearchTerms)
            {
                Dictionary<Webpage, HashSet<string>> terms = webpageSearchTerms.GetPrimary(entities);
                foreach (Webpage key in terms.Keys)
                {
                    if (primaryDictionary.ContainsKey(key))
                    {
                        primaryDictionary[key].AddRange(terms[key]);
                    }
                    primaryDictionary[key] = terms[key];
                }
            }
            var secondaryDictionary = new Dictionary<Webpage, HashSet<string>>();
            foreach (IGetWebpageSearchTerms webpageSearchTerms in _getWebpageSearchTerms)
            {
                Dictionary<Webpage, HashSet<string>> terms = webpageSearchTerms.GetSecondary(entities);
                foreach (Webpage key in terms.Keys)
                {
                    if (secondaryDictionary.ContainsKey(key))
                    {
                        secondaryDictionary[key].AddRange(terms[key]);
                    }
                    secondaryDictionary[key] = terms[key];
                }
            }
            return
                entities.Select(webpage =>
                    GetUniversalSearchItem(webpage,
                        primaryDictionary.ContainsKey(webpage) ? primaryDictionary[webpage] : new HashSet<string>(),
                        secondaryDictionary.ContainsKey(webpage) ? secondaryDictionary[webpage] : new HashSet<string>()
                        )).ToHashSet();
        }

        private UniversalSearchItem GetUniversalSearchItem(Webpage entity, HashSet<string> primarySearchTerms,
            IEnumerable<string> secondarySearchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/webpage/edit/" + entity.Id,
                Id = entity.Id,
                PrimarySearchTerms = primarySearchTerms,
                SecondarySearchTerms = secondarySearchTerms,
                SystemType = entity.GetType().FullName,
                CreatedOn = entity.CreatedOn
            };
        }
    }
}