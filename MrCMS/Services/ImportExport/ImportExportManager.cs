using System.IO;
using MrCMS.Services.ImportExport.DTOs;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Messages;
using MrCMS.Models;

namespace MrCMS.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentService;
        private readonly IExportDocumentsService _exportDocumentsService;
        private readonly IDocumentService _documentService;
        private readonly IMessageParser<ExportDocumentsEmailTemplate> _messageParser;

        public ImportExportManager(IImportDocumentsValidationService importDocumentsValidationService,
            IImportDocumentsService importDocumentsService, IExportDocumentsService exportDocumentsService,
            IDocumentService documentService, IMessageParser<ExportDocumentsEmailTemplate> messageParser)
        {
            _importDocumentsValidationService = importDocumentsValidationService;
            _importDocumentService = importDocumentsService;
            _exportDocumentsService = exportDocumentsService;
            _documentService = documentService;
            _messageParser = messageParser;
        }

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

        public byte[] ExportDocumentsToExcel()
        {
            var webpages = _documentService.GetAllDocuments<Webpage>().ToList();
            var package = _exportDocumentsService.GetExportExcelPackage(webpages);
            return _exportDocumentsService.ConvertPackageToByteArray(package);
        }

        public ExportDocumentsResult ExportDocumentsToEmail(ExportDocumentsModel model)
        {
            var queuedMessage = _messageParser.GetMessage(toAddress: model.Email);
            _messageParser.QueueMessage(queuedMessage, new List<AttachmentData>
            {
                new AttachmentData
                {
                    Data = ExportDocumentsToExcel(),
                    ContentType = XlsxContentType,
                    FileName = "Documents.xlsx"
                }
            });

            return new ExportDocumentsResult {Success = true};
        }

        public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}