using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Search.ItemCreation
{
    public interface IGetWebpageSearchTerms
    {
        IEnumerable<string> GetPrimary(Webpage webpage);
        Dictionary<Webpage,HashSet<string>> GetPrimary(HashSet<Webpage> webpages);
        IEnumerable<string> GetSecondary(Webpage webpage);
        Dictionary<Webpage,HashSet<string>> GetSecondary(HashSet<Webpage> webpages);
    }
}