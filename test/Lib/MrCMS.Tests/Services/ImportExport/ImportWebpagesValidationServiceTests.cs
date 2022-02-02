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
    public class ImportWebpagesValidationServiceTests
    {
        private readonly ImportWebpagesValidationService _importWebpagesValidationService;
        private IWebpageUrlService _webpageUrlService;
        private ServiceCollection _serviceCollection;
        private List<IWebpageImportValidationRule> _webpageImportValidationRules;

        public ImportWebpagesValidationServiceTests()
        {
            _webpageUrlService = A.Fake<IWebpageUrlService>();
            _serviceCollection = new ServiceCollection();
            _webpageImportValidationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IWebpageImportValidationRule>()).ToList();
            _webpageImportValidationRules.ForEach(rule =>
                _serviceCollection.AddTransient<IWebpageImportValidationRule>(provider => rule));
            _importWebpagesValidationService =
                new ImportWebpagesValidationService(_webpageUrlService, _serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllDocuments()
        {
            //var mockingKernel = new MockingKernel();
            //MrCMSKernel.OverrideKernel(mockingKernel);

            List<WebpageImportDTO> documents = Enumerable.Range(1, 10).Select(i => new WebpageImportDTO()).ToList();

            await _importWebpagesValidationService.ValidateBusinessLogic(documents);

            _webpageImportValidationRules.ForEach(
                rule =>
                    EnumerableHelper.ForEach(documents,
                        document => A.CallTo(() => rule.GetErrors(document, documents)).MustHaveHappened()));
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateImportFile_ShouldReturnNoErrors()
        {
            Dictionary<string, List<string>> errors =
                _importWebpagesValidationService.ValidateImportFile(GetSpreadsheet());

            errors.Count.Should().Be(0);
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocuments_ShouldReturnListOfDocumentsAndNoErrors()
        {
            var (items, errors) = await _importWebpagesValidationService.ValidateAndImportDocuments(GetSpreadsheet());

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
                await _importWebpagesValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.First().UrlSegment.Should().BeEquivalentTo("test-url");
            items.First().Name.Should().BeEquivalentTo("Test Document");
            items.First().BodyContent.Should().BeEquivalentTo("Test Body Content");
            items.First().MetaDescription.Should().BeEquivalentTo("Test SEO Description");
            items.First().MetaKeywords.Should().BeEquivalentTo("Test, Thought");
            items.First().MetaTitle.Should().BeEquivalentTo("Test SEO Title");
            items.First().DisplayOrder.Should().Be(2);
            items.First().RevealInNavigation.Should().BeTrue();
            items.First().PublishDate.Should().Be(currentTime);
            items.First().WebpageType.Should().BeEquivalentTo("StubWebpage");
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithTagsSet()
        {
            var (items, errors) = await _importWebpagesValidationService.ValidateAndImportDocuments(GetSpreadsheet());

            items.First().Tags.Should().HaveCount(1);
        }

        [Fact]
        public async Task
            ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithUrlHistorySet
            ()
        {
            var (items, errors) = await _importWebpagesValidationService.ValidateAndImportDocuments(GetSpreadsheet());

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

            var exportExcelPackage = new ExportWebpagesService().GetExportExcelPackage(items);

            return exportExcelPackage;
        }
    }
}