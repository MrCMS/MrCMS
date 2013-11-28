using System.IO;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Website;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentService;
        private readonly IExportDocumentsService _exportDocumentsService;
        private readonly IDocumentService _documentService;

        public ImportExportManager(IImportDocumentsValidationService importDocumentsValidationService,
            IImportDocumentsService importDocumentsService, IExportDocumentsService exportDocumentsService, IDocumentService documentService)
        {
            _importDocumentsValidationService = importDocumentsValidationService;
            _importDocumentService = importDocumentsService;
            _exportDocumentsService = exportDocumentsService;
            _documentService = documentService;
        }

        #region Import Documents
        public Dictionary<string, List<string>> ImportDocumentsFromExcel(Stream file)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            var items = GetDocumentsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return parseErrors;
            var businessLogicErrors = _importDocumentsValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return businessLogicErrors;
            _importDocumentService.ImportDocumentsFromDTOs(items);
            return new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private List<DocumentImportDTO> GetDocumentsFromSpreadSheet(ExcelPackage spreadsheet, out Dictionary<string, List<string>> parseErrors)
        {
            parseErrors = _importDocumentsValidationService.ValidateImportFile(spreadsheet);
            return parseErrors.Any()
                       ? new List<DocumentImportDTO>()
                       : _importDocumentsValidationService.ValidateAndImportDocuments(spreadsheet, ref parseErrors);
        }

        #endregion

        #region Export Documents
        public byte[] ExportDocumentsToExcel()
        {
            var webpages = _documentService.GetAllDocuments<Webpage>().ToList();
            var package = _exportDocumentsService.GetExportExcelPackage(webpages);
            return _exportDocumentsService.ConvertPackageToByteArray(package);
        }
        #endregion
    }
}