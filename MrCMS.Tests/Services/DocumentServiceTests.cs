using System;
using System.Linq;
using FakeItEasy;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using Xunit;
using FluentAssertions;

namespace MrCMS.Tests.Services
{
    public class DocumentServiceTests : InMemoryDatabaseTest
    {
        private readonly SiteSettings _siteSettings;

        public DocumentServiceTests()
        {
            _siteSettings = new SiteSettings();
        }
        [Fact]
        public void AddDocument_OnSave_AddsToRepository()
        {
            var documentService = GetDocumentService();

            documentService.AddDocument(new TextPage());

            Session.QueryOver<Document>().RowCount().Should().Be(1);
        }

        private DocumentService GetDocumentService()
        {
            var documentService = GetDocumentService();
            return documentService;
        }

        [Fact]
        public void GetDocument_WhenDocumentDoesNotExist_ReturnsNull()
        {
            var documentService = GetDocumentService();

            var document = documentService.GetDocument<TextPage>(1);

            document.Should().BeNull();
        }

        [Fact]
        public void DocumentService_SaveDocument_ReturnsTheSameDocument()
        {
            var documentService = GetDocumentService();

            var document = new TextPage();
            var updatedDocument = documentService.SaveDocument(document);

            document.Should().BeSameAs(updatedDocument);
        }

        [Fact]
        public void DocumentService_GetAllDocuments_ShouldReturnAListOfAllDocumentsOfTheSpecifiedType()
        {
            var documentService = GetDocumentService();

            Enumerable.Range(1, 10).ForEach(i => Session.Transact(session => session.SaveOrUpdate(new TextPage { Name = "Page " + i })));

            var allDocuments = documentService.GetAllDocuments<TextPage>();

            allDocuments.Should().HaveCount(10);
        }

        [Fact]
        public void DocumentService_GetAllDocuments_ShouldOnlyReturnDocumentsOfSpecifiedType()
        {
            var documentService = GetDocumentService();

            Enumerable.Range(1, 10).ForEach(i =>
                                         Session.Transact(
                                             session =>
                                             session.SaveOrUpdate(i % 2 == 0
                                                                      ? (Document)new TextPage { Name = "Page " + i }
                                                                      : new Layout { Name = "Layout " + i }
                                                 )));

            var allDocuments = documentService.GetAllDocuments<TextPage>();

            allDocuments.Should().HaveCount(5);
        }

        [Fact]
        public void DocumentService_GetDocumentsByParentId_ShouldReturnAllDocumentsThatHaveTheCorrespondingParentId()
        {
            var documentService = GetDocumentService();

            var parent = new TextPage { Name = "Parent" };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 10).ForEach(i =>
                                             {
                                                 var textPage = new TextPage { Name = String.Format("Page {0}", (object)i), Parent = parent };
                                                 parent.Children.Add(textPage);
                                                 Session.Transact(session => session.SaveOrUpdate(textPage));
                                                 Session.Transact(session => session.SaveOrUpdate(parent));
                                             });

            var documents = documentService.GetAdminDocumentsByParentId<TextPage>(parent.Id);

            documents.Should().HaveCount(10);
        }

        [Fact]
        public void DocumentService_GetDocumentsByParentId_ShouldOnlyReturnRequestedType()
        {
            var documentService = GetDocumentService();

            var parent = new TextPage { Name = "Parent" };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 10).ForEach(i =>
                                             {
                                                 var textPage = i % 2 == 0
                                                                    ? (Document)
                                                                      new TextPage { Name = String.Format("Page {0}", i), Parent = parent }
                                                                    : new Layout { Parent = parent };
                                                 parent.Children.Add(textPage);
                                                 Session.Transact(session => session.SaveOrUpdate(textPage));
                                                 Session.Transact(session => session.SaveOrUpdate(parent));
                                             });

            var textPages = documentService.GetAdminDocumentsByParentId<TextPage>(parent.Id);

            textPages.Should().HaveCount(5);
        }

        [Fact]
        public void DocumentService_GetDocumentsByParentId_ShouldOrderByDisplayOrder()
        {
            var documentService = GetDocumentService();

            var parent = new TextPage { Name = "Parent" };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 3).ForEach(i =>
                                             {
                                                 var textPage = new TextPage { Name = String.Format("Page {0}", i), Parent = parent, DisplayOrder = 4 - i };
                                                 parent.Children.Add(textPage);
                                                 Session.Transact(session => session.SaveOrUpdate(textPage));
                                             });

            var documents = documentService.GetAdminDocumentsByParentId<TextPage>(parent.Id).ToList();

            documents[0].DisplayOrder.Should().Be(1);
            documents[1].DisplayOrder.Should().Be(2);
            documents[2].DisplayOrder.Should().Be(3);
        }

        [Fact]
        public void DocumentService_GetDocumentByUrl_ReturnsTheDocumentWithTheSpecifiedUrl()
        {
            var documentService = GetDocumentService();

            var textPage = new TextPage { UrlSegment = "test-page" };
            Session.Transact(session => session.SaveOrUpdate(textPage));

            var document = documentService.GetDocumentByUrl<TextPage>("test-page");

            document.Should().NotBeNull();
        }

        [Fact]
        public void DocumentService_GetDocumentByUrl_ShouldReturnNullIfTheRequestedTypeDoesNotMatch()
        {
            var documentService = GetDocumentService();

            var textPage = new TextPage { UrlSegment = "test-page" };
            Session.Transact(session => session.SaveOrUpdate(textPage));

            var document = documentService.GetDocumentByUrl<Layout>("test-page");

            document.Should().BeNull();
        }

        [Fact]
        public void DocumentService_GetDocumentUrl_ReturnsAUrlBasedOnTheHierarchyIfTheFlagIsSetToTrue()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage { Name = "Test Page", UrlSegment = "test-page" };

            Session.Transact(session => session.SaveOrUpdate(textPage));

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage.Id, true);

            documentUrl.Should().Be("test-page/nested-page");
        }
        [Fact]
        public void DocumentService_GetDocumentUrl_ReturnsAUrlBasedOnTheNameIfTheFlagIsSetToFalse()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage { Name = "Test Page", UrlSegment = "test-page" };

            Session.Transact(session => session.SaveOrUpdate(textPage));

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage.Id, false);

            documentUrl.Should().Be("nested-page");
        }

        [Fact]
        public void DocumentService_GetDocumentUrlWithExistingName_ShouldReturnTheUrlWithADigitAppended()
        {
            var documentService = GetDocumentService();
            var parent = new TextPage { Name = "Parent", UrlSegment = "parent" };
            var textPage = new TextPage { Name = "Test Page", Parent = parent, UrlSegment = "parent/test-page" };
            var existingPage = new TextPage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page",
                                       Parent = textPage
                                   };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(existingPage);
            });

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage.Id, true);

            documentUrl.Should().Be("parent/test-page/nested-page-1");
        }

        [Fact]
        public void DocumentService_GetDocumentUrlWithExistingName_MultipleFilesWithSameNameShouldNotAppendMultipleDigits()
        {
            var documentService = GetDocumentService();
            var parent = new TextPage { Name = "Parent", UrlSegment = "parent" };
            var textPage = new TextPage { Name = "Test Page", Parent = parent, UrlSegment = "parent/test-page" };
            var existingPage = new TextPage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page",
                                       Parent = textPage
                                   };
            var existingPage2 = new TextPage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page-1",
                                       Parent = textPage
                                   };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(existingPage);
                session.SaveOrUpdate(existingPage2);
            });

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage.Id, true);

            documentUrl.Should().Be("parent/test-page/nested-page-2");
        }

        [Fact]
        public void DocumentService_GetLayoutId_IfWebpageIdIsInvalidReturnNull()
        {
            var documentService = GetDocumentService();

            var layoutId = documentService.GetLayoutId(0);

            layoutId.Should().Be(null);
        }

        [Fact]
        public void DocumentService_GetLayoutId_IfWebpageIdIsValidButNoLayoutIsSetReturnNull()
        {
            var documentService = GetDocumentService();

            var textpage = new TextPage();
            Session.Transact(session => session.SaveOrUpdate(textpage));

            var layoutId = documentService.GetLayoutId(textpage.Id);

            layoutId.Should().Be(null);
        }

        [Fact]
        public void DocumentService_GetLayoutId_IfWebpageIdIsValidAndLayoutIsSetReturnLayoutId()
        {
            var documentService = GetDocumentService();

            var layout = new Layout();
            var textpage = new TextPage() { Layout = layout };
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(textpage);
                                     session.SaveOrUpdate(layout);
                                 });

            var layoutId = documentService.GetLayoutId(textpage.Id);

            layoutId.Should().Be(layout.Id);
        }

        [Fact]
        public void DocumentService_SetTags_IfDocumentIsNullThrowArgumentNullException()
        {
            var documentService = GetDocumentService();

            documentService.Invoking(service => service.SetTags(null, null)).ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void DocumentService_SetTags_IfTagsIsNullForANewDocumentTheTagListShouldBeEmpty()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags(null, textPage);

            textPage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentService_SetTags_IfTagsHasOneStringTheTagListShouldHave1Tag()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test tag", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentService_SetTags_IfTagsHasTwoCommaSeparatedTagsTheTagListShouldHave2Tags()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldTrimTagNames()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test 1, test 2", textPage);

            textPage.Tags[1].Name.Should().Be("test 2");
        }

        [Fact]
        public void DocumentService_SetTags_ShouldSaveGeneratedTags()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test 1, test 2", textPage);

            Session.QueryOver<Tag>().RowCount().Should().Be(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldNotRecreateTags()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(textPage);
                                     session.SaveOrUpdate(tag1);
                                     session.SaveOrUpdate(tag2);
                                 });

            documentService.SetTags(textPage.TagList, textPage);

            Session.QueryOver<Tag>().RowCount().Should().Be(2);
        }
        [Fact]
        public void DocumentService_SetTags_ShouldNotReaddSetTags()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
            {
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(tag1);
                session.SaveOrUpdate(tag2);
            });

            documentService.SetTags(textPage.TagList, textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldRemoveTagsNotIncluded()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);

            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(textPage);
                                     session.SaveOrUpdate(tag1);
                                     session.SaveOrUpdate(tag2);
                                 });

            documentService.SetTags("test 1", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldAssignDocumentToTag()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            Session.Transact(session => session.SaveOrUpdate(textPage));

            documentService.SetTags("test 1", textPage);

            var tags = Session.QueryOver<Tag>().List();

            tags.Should().HaveCount(1);
            tags.First().Documents.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldRemoveTheDocumentFromTags()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();
            var tag1 = new Tag { Name = "test 1" };
            var tag2 = new Tag { Name = "test 2" };
            textPage.Tags.Add(tag1);
            textPage.Tags.Add(tag2);
            tag1.Documents.Add(textPage);
            tag2.Documents.Add(textPage);

            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(textPage);
                                     session.SaveOrUpdate(tag1);
                                     session.SaveOrUpdate(tag2);
                                 });

            documentService.SetTags("test 1", textPage);

            tag1.Documents.Should().HaveCount(1);
            tag2.Documents.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldNotCreateTagsWithEmptyNames()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test 1,,test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
            Session.QueryOver<Tag>().List().Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldNotCreateTagsWithEmptyNamesForTrailingComma()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            documentService.SetTags("test 1, test 2, ", textPage);

            textPage.Tags.Should().HaveCount(2);
            Session.QueryOver<Tag>().List().Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetOrder_ShouldSetTheDocumentOrderOfTheDocumentWithTheSetId()
        {
            var documentService = GetDocumentService();
            var textPage = new TextPage();

            Session.Transact(session => session.SaveOrUpdate(textPage));

            documentService.SetOrder(textPage.Id, 2);

            textPage.DisplayOrder.Should().Be(2);
        }

        //[Fact]
        //public void DocumentService_GetDefaultLayout_ShouldReturnNullIfIdIsNotSet()
        //{
        //    A.CallTo(() => _siteSettingsService.GetSettingValue<int?>(SiteSettings.DefaultLayoutKey)).Returns(null);
        //    var documentService = new DocumentService(Session, _siteSettingsService);

        //    var defaultLayout = documentService.GetDefaultLayout();

        //    defaultLayout.Should().BeNull();
        //}

        //[Fact]
        //public void DocumentService_GetDefaultLayout_ShouldReturnLayoutWithSiteSettingId()
        //{
        //    var layout = new Layout();
        //    Session.Transact(session => session.SaveOrUpdate(layout));
        //    A.CallTo(() => _siteSettingsService.GetSettingValue<int?>(SiteSettings.DefaultLayoutKey)).Returns(layout.Id);
        //    var documentService = new DocumentService(Session, _siteSettingsService);

        //    var defaultLayout = documentService.GetDefaultLayout();

        //    defaultLayout.Should().BeSameAs(layout);
        //}

        [Fact]
        public void DocumentService_SearchDocuments_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new TextPage { Name = "Test" };
            var doc2 = new TextPage { Name = "Different Name" };
            Session.Transact(session =>
                                 {
                                     session.SaveOrUpdate(doc1);
                                     session.SaveOrUpdate(doc2);
                                 });
            var documentService = GetDocumentService();

            var searchResultModels = documentService.SearchDocuments<TextPage>("Test");

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }
    }
}
