using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreLayoutSearchTerms : IGetLayoutSearchTerms
    {
        public IEnumerable<string> Get(Layout layout)
        {
            yield return layout.Name;
            yield return layout.UrlSegment;
        }

        public Dictionary<Layout, HashSet<string>> Get(HashSet<Layout> layouts)
        {
            return layouts.ToDictionary(layout => layout, layout => Get(layout).ToHashSet());
        }
    }
}