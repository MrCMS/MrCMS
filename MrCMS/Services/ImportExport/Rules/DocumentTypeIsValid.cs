using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public class DocumentTypeIsValid : IDocumentImportValidationRule
    {
        public IEnumerable<string> GetErrors(DocumentImportDataTransferObject item)
        {
            if (string.IsNullOrWhiteSpace(item.DocumentType) || DocumentMetadataHelper.GetTypeByName(item.DocumentType) == null)
                yield return "Document Type is not valid MrCMS type.";
        }
    }
}