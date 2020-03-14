using System.Collections.Generic;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public interface IDocumentImportValidationRule
    {
        IAsyncEnumerable<string> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems);
    }
}