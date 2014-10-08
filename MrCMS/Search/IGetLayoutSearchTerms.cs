using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;

namespace MrCMS.Search
{
    public interface IGetLayoutSearchTerms
    {
        IEnumerable<string> Get(Layout layout);
    }
}