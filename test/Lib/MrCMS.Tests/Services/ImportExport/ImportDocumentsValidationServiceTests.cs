using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Services.ImportExport.Rules;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsValidationServiceTests
    {
        private readonly ImportDocumentsValidationService _importDocumentsValidationService;
        private IWebpageUrlService _webpageUrlService;
        private ServiceCollection _serviceCollection;
        private List<IDocumentImportValidationRule> _documentImportValidationRules;

        public ImportDocumentsValidationServiceTests()
        {
            _webpageUrlService = A.Fake<IWebpageUrlService>();
            _serviceCollection = new ServiceCollection();
            _documentImportValidationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IDocumentImportValidationRule>()).ToList();
            _documentImportValidationRules.ForEach(rule =>
                _serviceCollection.AddTransient<IDocumentImportValidationRule>(provider => rule));
            _importDocumentsValidationService =
                new ImportDocumentsValidationService(_webpageUrlService, _serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllDocuments()
        {
            //var mockingKernel = new MockingKernel();
            //MrCMSKernel.OverrideKernel(mockingKernel);

            List<DocumentImportDTO> documents = Enumerable.Range(1, 10).Select(i => new DocumentImportDTO()).ToList();

            await _importDocumentsValidationService.ValidateBusinessLogic(documents);

            _documentImportValidationRules.ForEach(
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
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocuments_ShouldReturnListOfDocumentsAndNoErrors()
        {
            var (items, errors) = await _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.Count.Should().Be(1);
            errors.Count.Should().Be(0);
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithPrimaryPropertiesSet
            ()
        {
            DateTime currentTime = DateTime.Parse("2013-07-19 15:18:20");
            var (items, parseErrors) =
                await _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.First().UrlSegment.Should().BeEquivalentTo("test-url");
            items.First().Name.Should().BeEquivalentTo("Test Document");
            items.First().BodyContent.Should().BeEquivalentTo("Test Body Content");
            items.First().MetaDescription.Should().BeEquivalentTo("Test SEO Description");
            items.First().MetaKeywords.Should().BeEquivalentTo("Test, Thought");
            items.First().MetaTitle.Should().BeEquivalentTo("Test SEO Title");
            items.First().DisplayOrder.Should().Be(2);
            items.First().RevealInNavigation.Should().BeTrue();
            items.First().PublishDate.Should().Be(currentTime);
            items.First().DocumentType.Should().BeEquivalentTo("StubWebpage");
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithTagsSet()
        {
            var (items, errors) = await _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.First().Tags.Should().HaveCount(1);
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithUrlHistorySet
            ()
        {
            var (items, errors) = await _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.First().UrlHistory.Should().HaveCount(1);
        }

        private XLWorkbook GetSpreadsheet()
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
                PublishOn = currentTime
            };
            document.Tags.Add(new Tag {Id = 1, Name = "Test"});
            document.Urls.Add(new UrlHistory {UrlSegment = "test-url-old"});
            var items = new List<Webpage> {document};

            var exportExcelPackage = new ExportDocumentsService().GetExportExcelPackage(items);

            return exportExcelPackage;
        }
    }
}