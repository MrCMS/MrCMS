using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Search
{
    public interface IGetWebpageSearchTerms
    {
        IEnumerable<string> Get(Webpage webpage);
    }
}