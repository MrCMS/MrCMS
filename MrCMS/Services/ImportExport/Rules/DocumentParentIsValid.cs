using System.Collections.Generic;
using System.Linq;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentParentIsValid : IDocumentImportValidationRule
    {
        private readonly IGetDocumentByUrl<Webpage> _getWebpageByUrl;

        public DocumentParentIsValid(IGetDocumentByUrl<Webpage> getWebpageByUrl)
        {
            _getWebpageByUrl = getWebpageByUrl;
        }

        public IEnumerable<string> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            if (!string.IsNullOrWhiteSpace(item.ParentUrl) && _getWebpageByUrl.GetByUrl(item.ParentUrl) == null && !allItems.Any(x => x.UrlSegment == item.ParentUrl))
                yield return "The parent url specified is not present within the system.";
        }
    }
}