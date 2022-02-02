using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport.Rules
{
    public interface IWebpageImportValidationRule
    {
        Task<IReadOnlyList<string>> GetErrors(WebpageImportDTO item, IList<WebpageImportDTO> allItems);
    }
}