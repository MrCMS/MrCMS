using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Search.Models;
using StackExchange.Profiling;

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
            var searchTerms = _getWebpageSearchTerms.SelectMany(terms => terms.Get(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, searchTerms);
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<Webpage> entities)
        {
            Dictionary<Webpage, HashSet<string>> dictionary = new Dictionary<Webpage, HashSet<string>>();
            foreach (var webpageSearchTerms in _getWebpageSearchTerms)
            {
                using (MiniProfiler.Current.Step("Getting terms: " + webpageSearchTerms.GetType().Name))
                {
                    var terms = webpageSearchTerms.Get(entities);
                    foreach (var key in terms.Keys)
                    {
                        if (dictionary.ContainsKey(key))
                        {
                            dictionary[key].AddRange(terms[key]);
                        }
                        dictionary[key] = terms[key];
                    }
                }
            }
            using (MiniProfiler.Current.Step("Get core webpage items"))
            {
                return
                    entities.Select(webpage =>
                        GetUniversalSearchItem(webpage,
                            dictionary.ContainsKey(webpage) ? dictionary[webpage] : new HashSet<string>())).ToHashSet();
            }
        }

        private UniversalSearchItem GetUniversalSearchItem(Webpage entity, HashSet<string> searchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                ActionUrl = "/admin/webpage/edit/" + entity.Id, // removed UrlHelper for performance reasons
                Id = entity.Id,
                SearchTerms = searchTerms,
                SystemType = entity.GetType().FullName,
            };
        }
    }
}