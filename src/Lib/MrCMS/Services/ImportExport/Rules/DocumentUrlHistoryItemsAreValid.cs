using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentUrlHistoryItemsAreValid : IDocumentImportValidationRule
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;
        private readonly IUrlHistoryImportService _urlHistoryService;

        public DocumentUrlHistoryItemsAreValid(IGetDocumentByUrl<Webpage> getWebpageByUrl, IUrlHistoryImportService urlHistoryService)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _urlHistoryService = urlHistoryService;
        }

        public IEnumerable<string> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            if (item.UrlHistory.Count <= 0) yield break;

            var document = _getWebpageByUrl.GetByUrl(item.UrlSegment);

            if (document == null) yield break;

            var urls = _urlHistoryService.GetAllOtherUrls(document).ToList();
            foreach (var url in item.UrlHistory.Where(url => urls.Any(x => x.UrlSegment == url)))
            {
                yield return "One of url history segments is already within the system and belongs to another document.";
            }
        }
    }
}