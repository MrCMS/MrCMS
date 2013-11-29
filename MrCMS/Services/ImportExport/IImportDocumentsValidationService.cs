using System.Collections.Generic;
using MrCMS.Services.ImportExport.DTOs;
using OfficeOpenXml;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsValidationService
    {
        Dictionary<string, List<string>> ValidateBusinessLogic(IEnumerable<DocumentImportDTO> items);
        List<DocumentImportDTO> ValidateAndImportDocuments(ExcelPackage spreadsheet,
                                                                                    ref Dictionary<string, List<string>>
                                                                                        parseErrors);
        Dictionary<string, List<string>> ValidateImportFile(ExcelPackage spreadsheet);
    }
}