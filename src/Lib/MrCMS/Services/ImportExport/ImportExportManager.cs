using System;
using System.IO;
using MrCMS.Services.ImportExport.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Messages;
using MrCMS.Models;
using OfficeOpenXml;

namespace MrCMS.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentService;
        private readonly IExportDocumentsService _exportDocumentsService;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly ILogger<ImportExportManager> _logger;
        private readonly IMessageParser<ExportDocumentsEmailTemplate> _messageParser;

        public ImportExportManager(IImportDocumentsValidationService importDocumentsValidationService,
            IImportDocumentsService importDocumentsService, IExportDocumentsService exportDocumentsService,
            IMessageParser<ExportDocumentsEmailTemplate> messageParser, IRepository<Webpage> webpageRepository,
            ILogger<ImportExportManager> logger)
        {
            _importDocumentsValidationService = importDocumentsValidationService;
            _importDocumentService = importDocumentsService;
            _exportDocumentsService = exportDocumentsService;
            _messageParser = messageParser;
            _webpageRepository = webpageRepository;
            _logger = logger;
        }

        public async Task<ImportDocumentsResult> ImportDocumentsFromExcel(Stream file, bool autoStart = true)
        {
            var spreadsheet = new ExcelPackage(file);

            var (items, parseErrors) = await GetDocumentsFromSpreadSheet(spreadsheet);
            if (parseErrors.Any())
                return ImportDocumentsResult.Failure(parseErrors);
            var businessLogicErrors = await _importDocumentsValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return ImportDocumentsResult.Failure(businessLogicErrors);
            var batch = await _importDocumentService.CreateBatch(items, autoStart);
            //_importDocumentService.ImportDocumentsFromDTOs(items);
            return ImportDocumentsResult.Successful(batch);
        }


        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private async Task<(List<DocumentImportDTO>, Dictionary<string, List<string>>)> GetDocumentsFromSpreadSheet(ExcelPackage spreadsheet)
        {
            var parseErrors = _importDocumentsValidationService.ValidateImportFile(spreadsheet);
            return parseErrors.Any()
                       ? (new List<DocumentImportDTO>(), parseErrors)
                       : await _importDocumentsValidationService.ValidateAndImportDocuments(spreadsheet);
        }

        public byte[] ExportDocumentsToExcel()
        {
            try
            {
                var webpages = _webpageRepository.Query().ToList();
                var package = _exportDocumentsService.GetExportExcelPackage(webpages);
                return _exportDocumentsService.ConvertPackageToByteArray(package);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }

        public async Task<ExportDocumentsResult> ExportDocumentsToEmail(ExportDocumentsModel model)
        {
            try
            {
                var queuedMessage = await _messageParser.GetMessage(toAddress: model.Email);
                await _messageParser.QueueMessage(queuedMessage, new List<AttachmentData>
                {
                    new AttachmentData
                    {
                        Data = ExportDocumentsToExcel(),
                        ContentType = XlsxContentType,
                        FileName = "Documents.xlsx"
                    }
                });

                return new ExportDocumentsResult { Success = true };
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }

        public const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}