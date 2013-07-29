using System.Collections.Generic;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public interface IDocumentImportValidationRule
    {
        IEnumerable<string> GetErrors(DocumentImportDataTransferObject item, IList<DocumentImportDataTransferObject> allItems);
    }
}