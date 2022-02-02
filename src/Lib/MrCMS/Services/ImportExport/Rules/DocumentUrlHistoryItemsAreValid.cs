using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentUrlHistoryItemsAreValid : IDocumentImportValidationRule
    {
        private readonly IGetWebpageByUrl<Webpage> _getWebpageByUrl;
        private readonly IUrlHistoryImportService _urlHistoryService;

        public DocumentUrlHistoryItemsAreValid(IGetWebpageByUrl<Webpage> getWebpageByUrl,
            IUrlHistoryImportService urlHistoryService)
        {
            _getWebpageByUrl = getWebpageByUrl;
            _urlHistoryService = urlHistoryService;
        }

        public async Task<IReadOnlyList<string>> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            var list = new List<string>();
            if (item.UrlHistory.Count <= 0) return list;

            var document = await _getWebpageByUrl.GetByUrl(item.UrlSegment);

            if (document == null) return list;

            var urls = await _urlHistoryService.GetAllOtherUrls(document);
            list.AddRange(item.UrlHistory.Where(url => urls.Any(x => x.UrlSegment == url)).Select(url => $"One of url history segments is already within the system and belongs to another document for '{url}'."));

            return list;
        }
    }
}