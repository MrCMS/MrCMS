using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetLayoutSearchTerms
    {
        IEnumerable<string> Get(Layout layout);
    }
}