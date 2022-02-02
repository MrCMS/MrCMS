using System;
using System.IO;
using MrCMS.Services.ImportExport.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Messages;
using MrCMS.Models;
using NHibernate.Linq;

namespace MrCMS.Services.ImportExport
{
    public class ImportExportManager : IImportExportManager
    {
        private readonly IImportWebpagesValidationService _importWebpagesValidationService;
        private readonly IImportWebpagesService _importWebpageService;
        private readonly IExportWebpagesService _exportWebpagesService;
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly ILogger<ImportExportManager> _logger;
        private readonly IMessageParser<ExportWebpagesEmailTemplate> _messageParser;

        public ImportExportManager(IImportWebpagesValidationService importWebpagesValidationService,
            IImportWebpagesService importWebpagesService, IExportWebpagesService exportWebpagesService,
            IMessageParser<ExportWebpagesEmailTemplate> messageParser, IRepository<Webpage> webpageRepository,
            ILogger<ImportExportManager> logger)
        {
            _importWebpagesValidationService = importWebpagesValidationService;
            _importWebpageService = importWebpagesService;
            _exportWebpagesService = exportWebpagesService;
            _messageParser = messageParser;
            _webpageRepository = webpageRepository;
            _logger = logger;
        }

        public async Task<ImportWebpagesResult> ImportWebpagesFromExcel(Stream file, bool autoStart = true)
        {
            var spreadsheet = new XLWorkbook(file);

            var (items, parseErrors) = await GetWebpagesFromSpreadSheet(spreadsheet);
            if (parseErrors.Any())
                return ImportWebpagesResult.Failure(parseErrors);
            var businessLogicErrors = await _importWebpagesValidationService.ValidateBusinessLogic(items);
            if (businessLogicErrors.Any())
                return ImportWebpagesResult.Failure(businessLogicErrors);
            var batch = await _importWebpageService.CreateBatch(items, autoStart);
            //_importDocumentService.ImportDocumentsFromDTOs(items);
            return ImportWebpagesResult.Successful(batch);
        }


        /// <summary>
        /// Try and get data out of the spreadsheet into the DTOs with parse and type checks
        /// </summary>
        /// <param name="spreadsheet"></param>
        /// <param name="parseErrors"></param>
        /// <returns></returns>
        private async Task<(List<WebpageImportDTO> data, Dictionary<string, List<string>> parseErrors)>
            GetWebpagesFromSpreadSheet(XLWorkbook spreadsheet)
        {
            var parseErrors = _importWebpagesValidationService.ValidateImportFile(spreadsheet);
            if (parseErrors.Any())
                return (new List<WebpageImportDTO>(), parseErrors);
            return await _importWebpagesValidationService.ValidateAndImportDocuments(spreadsheet);
        }

        public async Task<byte[]> ExportWebpagesToExcel()
        {
            try
            {
                var webpages = await _webpageRepository.Query().ToListAsync();
                var package = _exportWebpagesService.GetExportExcelPackage(webpages);
                return _exportWebpagesService.ConvertPackageToByteArray(package);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }

        public async Task<ExportWebpagesResult> ExportWebpagesToEmail(ExportWebpagesModel model)
        {
            try
            {
                var queuedMessage = await _messageParser.GetMessage(toAddress: model.Email);
                await _messageParser.QueueMessage(queuedMessage, new List<AttachmentData>
                {
                    new AttachmentData
                    {
                        Data = await ExportWebpagesToExcel(),
                        ContentType = XlsxContentType,
                        FileName = "Documents.xlsx"
                    }
                });

                return new ExportWebpagesResult {Success = true};
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