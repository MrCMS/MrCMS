using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsValidationService
    {
        Task<Dictionary<string, List<string>>> ValidateBusinessLogic(IEnumerable<DocumentImportDTO> items);
        Task<(List<DocumentImportDTO> data,  Dictionary<string, List<string>> parseErrors)> ValidateAndImportDocuments(XLWorkbook spreadsheet);
        Dictionary<string, List<string>> ValidateImportFile(XLWorkbook spreadsheet);
    }
}