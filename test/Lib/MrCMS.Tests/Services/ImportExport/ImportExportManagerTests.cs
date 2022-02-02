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
        private readonly IImportWebpagesValidationService _importWebpagesValidationService;
        private readonly IImportWebpagesService _importWebpagesService;
        private readonly ImportExportManager _importExportManager;
        private readonly IExportWebpagesService _exportWebpagesService;

        private readonly IMessageParser<ExportWebpagesEmailTemplate> _messageParser =
            A.Fake<IMessageParser<ExportWebpagesEmailTemplate>>();

        private readonly InMemoryRepository<Webpage> _inMemoryRepository;
        private readonly ILogger<ImportExportManager> _logger;

        public ImportExportManagerTests()
        {
            _importWebpagesValidationService = A.Fake<IImportWebpagesValidationService>();
            _importWebpagesService = A.Fake<IImportWebpagesService>();

            _exportWebpagesService = A.Fake<IExportWebpagesService>();
            _inMemoryRepository = new InMemoryRepository<Webpage>();
            _logger = A.Fake<ILogger<ImportExportManager>>();
            _importExportManager = new ImportExportManager(_importWebpagesValidationService, _importWebpagesService, _exportWebpagesService, _messageParser, _inMemoryRepository, _logger);
        }

        [Fact]
        public void ImportExportManager_ExportDocumentsToExcel_ShouldReturnByteArray()
        {
            var result = _importExportManager.ExportWebpagesToExcel();

            result.Should().BeOfType<byte[]>();
        }
    }
}