using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetLayoutSearchTerms
    {
        IEnumerable<string> GetPrimary(Layout layout);
        Dictionary<Layout, HashSet<string>> GetPrimary(HashSet<Layout> layouts);
        IEnumerable<string> GetSecondary(Layout entity);
        Dictionary<Layout, HashSet<string>> GetSecondary(HashSet<Layout> layouts);
    }
}