using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public interface IDocumentImportValidationRule
    {
        Task<IReadOnlyList<string>> GetErrors(DocumentImportDTO item, IList<DocumentImportDTO> allItems);
    }
}