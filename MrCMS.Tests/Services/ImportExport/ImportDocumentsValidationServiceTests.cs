using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Services.ImportExport.Rules;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Ninject.MockingKernel;
using OfficeOpenXml;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsValidationServiceTests : InMemoryDatabaseTest
    {
        private readonly IDocumentService _documentService;
        private readonly ImportDocumentsValidationService _importDocumentsValidationService;
        private IWebpageUrlService _webpageUrlService;

        public ImportDocumentsValidationServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _webpageUrlService = A.Fake<IWebpageUrlService>();
            _importDocumentsValidationService = new ImportDocumentsValidationService(_documentService,
                _webpageUrlService);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllDocuments()
        {
            var mockingKernel = new MockingKernel();
            List<IDocumentImportValidationRule> documentImportValidationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IDocumentImportValidationRule>()).ToList();
            documentImportValidationRules.ForEach(rule => mockingKernel.Bind<IDocumentImportValidationRule>()
                .ToMethod(context => rule));
            MrCMSApplication.OverrideKernel(mockingKernel);

            List<DocumentImportDTO> documents = Enumerable.Range(1, 10).Select(i => new DocumentImportDTO()).ToList();

            _importDocumentsValidationService.ValidateBusinessLogic(documents);

            documentImportValidationRules.ForEach(
                rule =>
                    EnumerableHelper.ForEach(documents,
                        document => A.CallTo(() => rule.GetErrors(document, documents)).MustHaveHappened()));
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateImportFile_ShouldReturnNoErrors()
        {
            Dictionary<string, List<string>> errors =
                _importDocumentsValidationService.ValidateImportFile(GetSpreadsheet());

            errors.Count.Should().Be(0);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateAndImportDocuments_ShouldReturnListOfDocumentsAndNoErrors()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            List<DocumentImportDTO> items =
                _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.Count.Should().Be(1);
            parseErrors.Count.Should().Be(0);
        }

        [Fact]
        public void
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithPrimaryPropertiesSet
            ()
        {
            DateTime currentTime = DateTime.Parse("2013-07-19 15:18:20");
            var parseErrors = new Dictionary<string, List<string>>();
            List<DocumentImportDTO> items =
                _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.First().UrlSegment.Should().BeEquivalentTo("test-url");
            items.First().Name.Should().BeEquivalentTo("Test Document");
            items.First().BodyContent.Should().BeEquivalentTo("Test Body Content");
            items.First().MetaDescription.Should().BeEquivalentTo("Test SEO Description");
            items.First().MetaKeywords.Should().BeEquivalentTo("Test, Thought");
            items.First().MetaTitle.Should().BeEquivalentTo("Test SEO Title");
            items.First().DisplayOrder.Should().Be(2);
            items.First().RevealInNavigation.Should().BeTrue();
            items.First().RequireSSL.Should().BeFalse();
            items.First().PublishDate.Should().Be(currentTime);
            items.First().DocumentType.Should().BeEquivalentTo("StubWebpage");
        }

        [Fact]
        public void
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithTagsSet()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            List<DocumentImportDTO> items =
                _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.First().Tags.Should().HaveCount(1);
        }

        [Fact]
        public void
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithUrlHistorySet
            ()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            List<DocumentImportDTO> items =
                _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.First().UrlHistory.Should().HaveCount(1);
        }

        private ExcelPackage GetSpreadsheet()
        {
            DateTime currentTime = DateTime.Parse("2013-07-19 15:18:20");
            var document = new StubWebpage
            {
                UrlSegment = "test-url",
                Name = "Test Document",
                BodyContent = "Test Body Content",
                MetaDescription = "Test SEO Description",
                MetaKeywords = "Test, Thought",
                MetaTitle = "Test SEO Title",
                DisplayOrder = 2,
                RevealInNavigation = true,
                RequiresSSL = false,
                PublishOn = currentTime
            };
            document.Tags.Add(new Tag {Id = 1, Name = "Test"});
            document.Urls.Add(new UrlHistory {UrlSegment = "test-url-old"});
            var items = new List<Webpage> {document};

            ExcelPackage exportExcelPackage = new ExportDocumentsService().GetExportExcelPackage(items);

            return exportExcelPackage;
        }
    }
}