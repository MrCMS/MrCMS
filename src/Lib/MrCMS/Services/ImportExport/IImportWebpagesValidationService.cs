using System.Collections.Generic;
using System.Threading.Tasks;
using ClosedXML.Excel;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IImportWebpagesValidationService
    {
        Task<Dictionary<string, List<string>>> ValidateBusinessLogic(IEnumerable<WebpageImportDTO> items);
        Task<(List<WebpageImportDTO> data,  Dictionary<string, List<string>> parseErrors)> ValidateAndImportDocuments(int siteId, XLWorkbook spreadsheet);
        Dictionary<string, List<string>> ValidateImportFile(XLWorkbook spreadsheet);
    }
}