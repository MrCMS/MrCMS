using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentUrlHistoryItemsAreValid : IDocumentImportValidationRule
    {
        private readonly IDocumentService _documentService;
        private readonly IUrlHistoryImportService _urlHistoryService;

        public DocumentUrlHistoryItemsAreValid(IDocumentService documentService,
            IUrlHistoryImportService urlHistoryService)
        {
            _documentService = documentService;
            _urlHistoryService = urlHistoryService;
        }

        public IEnumerable<string> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            if (item.UrlHistory.Count <= 0) yield break;

            var document = _documentService.GetDocumentByUrl<Webpage>(item.UrlSegment);

            if (document == null) yield break;

            List<UrlHistoryInfo> urls = _urlHistoryService.GetAllOtherUrls(document).ToList();
            foreach (string url in item.UrlHistory.Where(url => urls.Any(x => x.UrlSegment == url)))
            {
                yield return "One of url history segments is already within the system and belongs to another document."
                    ;
            }
        }
    }
}