using System.IO;
using MrCMS.Batching.Entities;
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
        public ImportDocumentsResult ImportDocumentsFromExcel(Stream file, bool autoStart = true)
        {
            var spreadsheet = new ExcelPackage(file);

            Dictionary<string, List<string>> parseErrors;
            var items = GetDocumentsFromSpreadSheet(spreadsheet, out parseErrors);
            if (parseErrors.Any())
                return ImportDocumentsResult.Failure(parseErrors);
            var businessLogicErrors = _importDocumentsValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return ImportDocumentsResult.Failure(businessLogicErrors);
            var batch = _importDocumentService.CreateBatch(items, autoStart);
            //_importDocumentService.ImportDocumentsFromDTOs(items);
            return ImportDocumentsResult.Successful(batch);
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

    public class ImportDocumentsResult
    {
        private ImportDocumentsResult()
        {
            Errors = new Dictionary<string, List<string>>();
        }
        public Batch Batch { get; private set; }
        public Dictionary<string, List<string>> Errors { get; private set; }
        public bool Success { get { return Batch != null; } }

        public static ImportDocumentsResult Successful(Batch batch)
        {
            return new ImportDocumentsResult { Batch = batch };
        }
        public static ImportDocumentsResult Failure(Dictionary<string, List<string>> errors)
        {
            return new ImportDocumentsResult { Errors = errors };
        }
    }
}