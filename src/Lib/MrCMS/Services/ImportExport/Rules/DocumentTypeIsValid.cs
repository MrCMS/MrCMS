using System.Collections.Generic;
using System.Threading.Tasks;
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

        public Task<IReadOnlyList<string>> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(item.DocumentType) ||
                _documentMetadataService.GetTypeByName(item.DocumentType) == null)
                list.Add("Document Type is not valid MrCMS type.");
            return Task.FromResult<IReadOnlyList<string>>(list);
        }
    }
}