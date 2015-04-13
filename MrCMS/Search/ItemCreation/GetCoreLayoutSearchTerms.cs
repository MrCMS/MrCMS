using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreLayoutSearchTerms : IGetLayoutSearchTerms
    {
        public IEnumerable<string> GetPrimary(Layout layout)
        {
            yield return layout.Name;
        }

        public Dictionary<Layout, HashSet<string>> GetPrimary(HashSet<Layout> layouts)
        {
            return layouts.ToDictionary(layout => layout, layout => GetPrimary(layout).ToHashSet());
        }

        public IEnumerable<string> GetSecondary(Layout layout)
        {
            yield return layout.UrlSegment;
        }

        public Dictionary<Layout, HashSet<string>> GetSecondary(HashSet<Layout> layouts)
        {
            return layouts.ToDictionary(layout => layout, layout => GetSecondary(layout).ToHashSet());
        }
    }
}