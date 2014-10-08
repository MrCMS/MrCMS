using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreWebpageSearchTerms : IGetWebpageSearchTerms
    {
        public IEnumerable<string> Get(Webpage webpage)
        {
            yield return webpage.Name;
            yield return webpage.BodyContent;
            yield return webpage.UrlSegment;
            foreach (Tag tag in webpage.Tags)
            {
                yield return tag.Name;
            }
        }
    }
}