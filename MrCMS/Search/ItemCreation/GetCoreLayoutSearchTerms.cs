using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreLayoutSearchTerms : IGetLayoutSearchTerms
    {
        public IEnumerable<string> Get(Layout layout)
        {
            yield return layout.Name;
            yield return layout.UrlSegment;
        }
    }
}