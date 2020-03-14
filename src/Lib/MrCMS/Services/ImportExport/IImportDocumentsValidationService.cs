using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Services.ImportExport.DTOs;
using OfficeOpenXml;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsValidationService
    {
        Task<Dictionary<string, List<string>>> ValidateBusinessLogic(IEnumerable<DocumentImportDTO> items);

        Task<(List<DocumentImportDTO>, Dictionary<string, List<string>>)> ValidateAndImportDocuments(
            ExcelPackage spreadsheet);
            
        Dictionary<string, List<string>> ValidateImportFile(ExcelPackage spreadsheet);
    }
}