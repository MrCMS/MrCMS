using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentTypeIsValid : IDocumentImportValidationRule
    {
        private readonly IDocumentMetadataService _documentMetadataService;

        public DocumentTypeIsValid(IDocumentMetadataService documentMetadataService)
        {
            _documentMetadataService = documentMetadataService;
        }

        public IEnumerable<string> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            if (string.IsNullOrWhiteSpace(item.DocumentType) ||
                _documentMetadataService.GetTypeByName(item.DocumentType) == null)
                yield return "Document Type is not valid MrCMS type.";
        }
    }
}