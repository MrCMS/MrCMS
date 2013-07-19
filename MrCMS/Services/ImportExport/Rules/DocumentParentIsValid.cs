using System.Collections.Generic;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentParentIsValid : IDocumentImportValidationRule
    {
        private readonly IDocumentService _documentService;

        public DocumentParentIsValid(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        public IEnumerable<string> GetErrors(DocumentImportDataTransferObject item)
        {
            if (!string.IsNullOrWhiteSpace(item.ParentUrl) && _documentService.GetDocumentByUrl<Webpage>(item.ParentUrl) == null)
                yield return "The parent url specified is not present within the system.";
        }
    }
}