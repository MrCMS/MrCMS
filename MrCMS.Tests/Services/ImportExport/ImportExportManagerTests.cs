using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using Xunit;
using MrCMS.Entities.Documents.Web;
using MrCMS.Messages;
using MrCMS.Tests.TestSupport;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportExportManagerTests
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentsService;
        private readonly ImportExportManager _importExportManager;
        private readonly IExportDocumentsService _exportDocumentsService;

        private IMessageParser<ExportDocumentsEmailTemplate> _messageParser =
            A.Fake<IMessageParser<ExportDocumentsEmailTemplate>>();

        private InMemoryRepository<Webpage> _inMemoryRepository;

        public ImportExportManagerTests()
        {
            _importDocumentsValidationService = A.Fake<IImportDocumentsValidationService>();
            _importDocumentsService = A.Fake<IImportDocumentsService>();

            _exportDocumentsService = A.Fake<IExportDocumentsService>();
            _inMemoryRepository = new InMemoryRepository<Webpage>();
            _importExportManager = new ImportExportManager(_importDocumentsValidationService, _importDocumentsService, _exportDocumentsService, _messageParser, _inMemoryRepository);
        }

        [Fact]
        public void ImportExportManager_ExportDocumentsToExcel_ShouldReturnByteArray()
        {
            var result = _importExportManager.ExportDocumentsToExcel();

            result.Should().BeOfType<byte[]>();
        }
    }
}