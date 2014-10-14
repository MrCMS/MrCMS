using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Search.ItemCreation
{
    public class GetCoreWebpageSearchTerms : IGetWebpageSearchTerms
    {
        private readonly IDocumentService _documentService;

        public GetCoreWebpageSearchTerms(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public IEnumerable<string> Get(Webpage webpage)
        {
            yield return webpage.Name;
            yield return webpage.BodyContent;
            yield return webpage.UrlSegment;
            IList<Tag> documentTags = _documentService.GetDocumentTags(webpage);
            foreach (Tag tag in documentTags)
            {
                yield return tag.Name;
            }
        }
    }
}