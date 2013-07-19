using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FakeItEasy;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Services.ImportExport.Rules;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using Ninject.MockingKernel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Xunit;
using FluentAssertions;
namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsValidationServiceTests : InMemoryDatabaseTest
    {
        private IDocumentService _documentService;
        private ImportDocumentsValidationService _importDocumentsValidationService;
       
        public ImportDocumentsValidationServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _importDocumentsValidationService= new ImportDocumentsValidationService(_documentService);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateBusinessLogic_CallsAllIoCRegisteredRulesOnAllDocuments()
        {
            var mockingKernel = new MockingKernel();
            var documentImportValidationRules =
                Enumerable.Range(1, 10).Select(i => A.Fake<IDocumentImportValidationRule>()).ToList();
            documentImportValidationRules.ForEach(rule => mockingKernel.Bind<IDocumentImportValidationRule>()
                                                                      .ToMethod(context => rule));
            MrCMSApplication.OverrideKernel(mockingKernel);

            var documents = Enumerable.Range(1, 10).Select(i => new DocumentImportDataTransferObject()).ToList();

            _importDocumentsValidationService.ValidateBusinessLogic(documents);

            documentImportValidationRules.ForEach(
                rule =>
                EnumerableHelper.ForEach(documents, document => A.CallTo(() => rule.GetErrors(document)).MustHaveHappened()));
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateImportFile_ShouldReturnNoErrors()
        {
            var errors = _importDocumentsValidationService.ValidateImportFile(GetSpreadsheet());

            errors.Count.Should().Be(0);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateAndImportDocuments_ShouldReturnListOfDocumentsAndNoErrors()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.Count.Should().Be(1);
            parseErrors.Count.Should().Be(0);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithPrimaryPropertiesSet()
        {
            var currentTime = DateTime.Parse("2013-07-19 15:18:20");
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

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
            items.First().DocumentType.Should().BeEquivalentTo("Article");
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithTagsSet()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.First().Tags.Should().HaveCount(1);
        }

        [Fact]
        public void ImportDocumentsValidationService_ValidateAndImportDocumentsWithVariants_ShouldReturnDocumentWithUrlHistorySet()
        {
            var parseErrors = new Dictionary<string, List<string>>();
            var items = _importDocumentsValidationService.ValidateAndImportDocuments(GetSpreadsheet(), ref parseErrors);

            items.First().UrlHistory.Should().HaveCount(1);
        }

        private ExcelPackage GetSpreadsheet()
        {
            var currentTime = DateTime.Parse("2013-07-19 15:18:20");
            var document = new Article()
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
            var items = new List<Webpage>()
                {
                    document
                };
            items[0].Tags.Add(new Tag(){Id=1, Name = "Test"});

            using (var excelFile = new ExcelPackage())
            {
                var wsInfo = excelFile.Workbook.Worksheets.Add("Info");

                wsInfo.Cells["A1:D1"].Style.Font.Bold = true;
                wsInfo.Cells["A:D"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsInfo.Cells["A1"].Value = "MrCMS Version";
                wsInfo.Cells["B1"].Value = "Entity Type for Export";
                wsInfo.Cells["C1"].Value = "Export Date";
                wsInfo.Cells["D1"].Value = "Export Source";

                wsInfo.Cells["A2"].Value = MrCMSHtmlHelper.AssemblyVersion(null);
                wsInfo.Cells["B2"].Value = "Webpage";
                wsInfo.Cells["C2"].Style.Numberformat.Format = "YYYY-MM-DDThh:mm:ss.sTZD";
                wsInfo.Cells["C2"].Value = DateTime.UtcNow;
                wsInfo.Cells["D2"].Value = "MrCMS " + MrCMSHtmlHelper.AssemblyVersion(null);

                wsInfo.Cells["A:D"].AutoFitColumns();
                wsInfo.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                wsInfo.Cells["A4"].Value = "Please do not change any values inside this file.";

                var wsItems = excelFile.Workbook.Worksheets.Add("Items");

                wsItems.Cells["A1:N1"].Style.Font.Bold = true;
                wsItems.Cells["A:N"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                wsItems.Cells["A1"].Value = "Url (Must not be changed!)";
                wsItems.Cells["B1"].Value = "Parent Url";
                wsItems.Cells["C1"].Value = "Document Type";
                wsItems.Cells["D1"].Value = "Name";
                wsItems.Cells["E1"].Value = "Body Content";
                wsItems.Cells["F1"].Value = "SEO Title";
                wsItems.Cells["G1"].Value = "SEO Description";
                wsItems.Cells["H1"].Value = "SEO Keywords";
                wsItems.Cells["I1"].Value = "Tags";
                wsItems.Cells["J1"].Value = "Reveal in navigation";
                wsItems.Cells["K1"].Value = "Display Order";
                wsItems.Cells["L1"].Value = "Require SSL";
                wsItems.Cells["M1"].Value = "Publish Date";
                wsItems.Cells["N1"].Value = "Url History";

                for (var i = 0; i < items.Count; i++)
                {
                    var rowId = i + 2;
                    wsItems.Cells["A" + rowId].Value = items[i].UrlSegment;
                    wsItems.Cells["A" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["B" + rowId].Value = items[i].Parent != null ? items[i].Parent.UrlSegment : String.Empty;
                    wsItems.Cells["B" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["C" + rowId].Value = items[i].DocumentType;
                    wsItems.Cells["D" + rowId].Value = items[i].Name;
                    wsItems.Cells["D" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["E" + rowId].Value = items[i].BodyContent;
                    wsItems.Cells["E" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["F" + rowId].Value = items[i].MetaTitle;
                    wsItems.Cells["G" + rowId].Value = items[i].MetaDescription;
                    wsItems.Cells["G" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                    wsItems.Cells["H" + rowId].Value = items[i].MetaKeywords;
                    for (var j = 0; j < items[i].Tags.Count; j++)
                    {
                        wsItems.Cells["I" + rowId].Value += items[i].Tags[j].Name;
                        if (j != items[i].Tags.Count - 1)
                            wsItems.Cells["I" + rowId].Value += ",";
                    }
                    wsItems.Cells["I" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    wsItems.Cells["J" + rowId].Value = items[i].RevealInNavigation;
                    wsItems.Cells["K" + rowId].Value = items[i].DisplayOrder;
                    wsItems.Cells["L" + rowId].Value = items[i].RequiresSSL;
                    wsItems.Cells["M" + rowId].Value = items[i].PublishOn.HasValue ? items[i].PublishOn.Value.ToString("yyyy-MM-dd HH:mm:ss") : String.Empty;
                    wsItems.Cells["N" + rowId].Value = "test-url-old";
                    wsItems.Cells["N" + rowId].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                }
                wsItems.Cells["A:D"].AutoFitColumns();
                wsItems.Cells["F:F"].AutoFitColumns();
                wsItems.Cells["H:N"].AutoFitColumns();

                return excelFile;
            }
        }
    }
}