using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MrCMS.Entities.Documents.Web;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportExportManagerTests
    {
        //private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        //private readonly IImportDocumentsService _importDocumentsService;
        //private readonly ImportExportManager _importExportManager;
        //private readonly IExportDocumentsService _exportDocumentsService;

        //private readonly IMessageParser<ExportDocumentsEmailTemplate> _messageParser =
        //    A.Fake<IMessageParser<ExportDocumentsEmailTemplate>>();

        //private readonly InMemoryRepository<Webpage> _inMemoryRepository;
        //private readonly ILogger<ImportExportManager> _logger;

        //public ImportExportManagerTests()
        //{
        //    _importDocumentsValidationService = A.Fake<IImportDocumentsValidationService>();
        //    _importDocumentsService = A.Fake<IImportDocumentsService>();

        //    _exportDocumentsService = A.Fake<IExportDocumentsService>();
        //    _inMemoryRepository = new InMemoryRepository<Webpage>();
        //    _logger = A.Fake<ILogger<ImportExportManager>>();
        //    _importExportManager = new ImportExportManager(_importDocumentsValidationService, _importDocumentsService, _exportDocumentsService, _messageParser, _inMemoryRepository, _logger);
        //}

        //[Fact]
        //public void ImportExportManager_ExportDocumentsToExcel_ShouldReturnByteArray()
        //{
        //    var result = _importExportManager.ExportDocumentsToExcel();

        //    result.Should().BeOfType<byte[]>();
        //}
    }
}