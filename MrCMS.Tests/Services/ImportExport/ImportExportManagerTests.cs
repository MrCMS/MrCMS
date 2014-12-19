using System.IO;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using Xunit;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportExportManagerTests : InMemoryDatabaseTest
    {
        private readonly IImportDocumentsValidationService _importDocumentsValidationService;
        private readonly IImportDocumentsService _importDocumentsService;
        private readonly IDocumentService _documentService;
        private readonly ImportExportManager _importExportManager;
        private readonly IExportDocumentsService _exportDocumentsService;

        public ImportExportManagerTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _importDocumentsValidationService = A.Fake<IImportDocumentsValidationService>();
            _importDocumentsService = A.Fake<IImportDocumentsService>();

            _exportDocumentsService = A.Fake<IExportDocumentsService>();
            _importExportManager = new ImportExportManager(_importDocumentsValidationService, _importDocumentsService, _exportDocumentsService, _documentService);
        }

        [Fact]
        public void ImportExportManager_ExportDocumentsToExcel_ShouldReturnByteArray()
        {
            var result = _importExportManager.ExportDocumentsToExcel();

            result.Should().BeOfType<byte[]>();
        }

        [Fact]
        public void ImportExportManager_ExportDocumentsToExcel_ShouldCallGetAllOfDocumentVariantService()
        {
            _importExportManager.ExportDocumentsToExcel();

            A.CallTo(() => _documentService.GetAllDocuments<Webpage>()).MustHaveHappened();
        }
    }
}