using System;
using System.Collections.Generic;
using System.Linq;
using Elmah;
using FakeItEasy;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using NHibernate;
using Xunit;
using FluentAssertions;

namespace MrCMS.Tests.Services
{
    public class DocumentServiceTests : InMemoryDatabaseTest
    {
        private readonly SiteSettings _siteSettings;
        private CurrentSite currentSite;


        public DocumentServiceTests()
        {
            _siteSettings = new SiteSettings();
        }

        [Fact]
        public void AddDocument_OnSave_AddsToRepository()
        {
            var documentService = GetDocumentService();

            documentService.AddDocument(new BasicMappedWebpage { Site = CurrentSite });

            Session.QueryOver<Document>().RowCount().Should().Be(1);
        }

        private DocumentService GetDocumentService(ISession session = null)
        {
            currentSite = new CurrentSite(CurrentSite);
            var documentService = new DocumentService(session ?? Session, _siteSettings, currentSite);
            return documentService;
        }

        [Fact]
        public void GetDocument_WhenDocumentDoesNotExist_ReturnsNull()
        {
            var documentService = GetDocumentService();

            var document = documentService.GetDocument<BasicMappedWebpage>(1);

            document.Should().BeNull();
        }

        [Fact]
        public void DocumentService_SaveDocument_ReturnsTheSameDocument()
        {
            var documentService = GetDocumentService();

            var document = new BasicMappedWebpage();
            var updatedDocument = documentService.SaveDocument(document);

            document.Should().BeSameAs(updatedDocument);
        }

        [Fact]
        public void DocumentService_GetAllDocuments_ShouldReturnAListOfAllDocumentsOfTheSpecifiedType()
        {
            var documentService = GetDocumentService();

            Enumerable.Range(1, 10).ForEach(i => Session.Transact(session => session.SaveOrUpdate(new BasicMappedWebpage { Name = "Page " + i })));

            var allDocuments = documentService.GetAllDocuments<BasicMappedWebpage>();

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
                                                                      ? (Document)new BasicMappedWebpage { Name = "Page " + i }
                                                                      : new Layout { Name = "Layout " + i }
                                                 )));

            var allDocuments = documentService.GetAllDocuments<BasicMappedWebpage>();

            allDocuments.Should().HaveCount(5);
        }

        [Fact]
        public void DocumentService_GetAdminDocumentsByParentId_ShouldReturnAllDocumentsThatHaveTheCorrespondingParentId()
        {
            var documentService = GetDocumentService();

            var parent = new BasicMappedWebpage
            {
                Name = "Parent",
                AdminAllowedRoles = new List<UserRole>()
            };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 10).ForEach(i =>
                                                {
                                                    var textPage = new BasicMappedWebpage
                                                                       {
                                                                           Name = String.Format("Page {0}", (object)i),
                                                                           Parent = parent,
                                                                           AdminAllowedRoles =
                                                                               new List<UserRole>(),
                                                                           Site = CurrentSite
                                                                       };
                                                    parent.Children.Add(textPage);
                                                    Session.Transact(session => session.SaveOrUpdate(textPage));
                                                    Session.Transact(session => session.SaveOrUpdate(parent));
                                                });

            var documents = documentService.GetAdminDocumentsByParent(parent);

            documents.Should().HaveCount(10);
        }

        [Fact]
        public void DocumentService_GetAdminDocumentsByParentId_ShouldOnlyReturnRequestedType()
        {
            var documentService = GetDocumentService();

            var parent = new BasicMappedWebpage
                             {
                                 Name = "Parent",
                                 AdminAllowedRoles = new List<UserRole>(),
                             };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 10).ForEach(i =>
                                             {
                                                 var textPage = i % 2 == 0
                                                                    ? (Document)
                                                                      new BasicMappedWebpage
                                                                      {
                                                                          Name = String.Format("Page {0}", i),
                                                                          Parent = parent,
                                                                          AdminAllowedRoles = new List<UserRole>(),
                                                                          Site = CurrentSite
                                                                      }
                                                                    : new Layout { Parent = parent };
                                                 parent.Children.Add(textPage);
                                                 Session.Transact(session => session.SaveOrUpdate(textPage));
                                                 Session.Transact(session => session.SaveOrUpdate(parent));
                                             });

            var textPages = documentService.GetAdminDocumentsByParent(parent);

            textPages.Should().HaveCount(5);
        }

        [Fact]
        public void DocumentService_GetAdminDocumentsByParentId_ShouldOrderByDisplayOrder()
        {
            var documentService = GetDocumentService();

            var parent = new BasicMappedWebpage
                             {
                                 Name = "Parent",
                                 AdminAllowedRoles = new List<UserRole>(),
                             };
            Session.Transact(session => session.SaveOrUpdate(parent));

            Enumerable.Range(1, 3).ForEach(i =>
                                               {
                                                   var textPage = new BasicMappedWebpage
                                                                      {
                                                                          Name = String.Format("Page {0}", i),
                                                                          Parent = parent,
                                                                          DisplayOrder = 4 - i,
                                                                          AdminAllowedRoles = new List<UserRole>(),
                                                                          Site = CurrentSite
                                                                      };
                                                   parent.Children.Add(textPage);
                                                   Session.Transact(session => session.SaveOrUpdate(textPage));
                                               });

            var documents = documentService.GetAdminDocumentsByParent<BasicMappedWebpage>(parent).ToList();

            documents[0].DisplayOrder.Should().Be(1);
            documents[1].DisplayOrder.Should().Be(2);
            documents[2].DisplayOrder.Should().Be(3);
        }

        [Fact]
        public void DocumentService_GetDocumentByUrl_ReturnsTheDocumentWithTheSpecifiedUrl()
        {
            var documentService = GetDocumentService();

            var textPage = new BasicMappedWebpage { UrlSegment = "test-page", Site = CurrentSite };
            Session.Transact(session => session.SaveOrUpdate(textPage));

            var document = documentService.GetDocumentByUrl<BasicMappedWebpage>("test-page");

            document.Should().NotBeNull();
        }

        [Fact]
        public void DocumentService_GetDocumentByUrl_ShouldReturnNullIfTheRequestedTypeDoesNotMatch()
        {
            var documentService = GetDocumentService();

            Site site = new Site();
            var textPage = new BasicMappedWebpage { UrlSegment = "test-page", Site = site };
            Session.Transact(session => session.SaveOrUpdate(textPage));

            var document = documentService.GetDocumentByUrl<Layout>("test-page");

            document.Should().BeNull();
        }

        [Fact]
        public void DocumentService_GetDocumentUrl_ReturnsAUrlBasedOnTheHierarchyIfTheFlagIsSetToTrue()
        {
            var documentService = GetDocumentService();
            Site site = new Site();
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = site };

            Session.Transact(session => session.SaveOrUpdate(textPage));

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage, true);

            documentUrl.Should().Be("test-page/nested-page");
        }
        [Fact]
        public void DocumentService_GetDocumentUrl_ReturnsAUrlBasedOnTheNameIfTheFlagIsSetToFalse()
        {
            var documentService = GetDocumentService();
            Site site = new Site();
            var textPage = new BasicMappedWebpage { Name = "Test Page", UrlSegment = "test-page", Site = site };

            Session.Transact(session => session.SaveOrUpdate(textPage));

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage, false);

            documentUrl.Should().Be("nested-page");
        }

        [Fact]
        public void DocumentService_GetDocumentUrlWithExistingName_ShouldReturnTheUrlWithADigitAppended()
        {
            var documentService = GetDocumentService();
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = CurrentSite };
            var textPage = new BasicMappedWebpage { Name = "Test Page", Parent = parent, UrlSegment = "parent/test-page", Site = CurrentSite };
            var existingPage = new BasicMappedWebpage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page",
                                       Parent = textPage,
                                       Site = CurrentSite
                                   };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(existingPage);
            });

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage, true);

            documentUrl.Should().Be("parent/test-page/nested-page-1");
        }

        [Fact]
        public void DocumentService_GetDocumentUrlWithExistingName_MultipleFilesWithSameNameShouldNotAppendMultipleDigits()
        {
            var documentService = GetDocumentService();
            var parent = new BasicMappedWebpage { Name = "Parent", UrlSegment = "parent", Site = CurrentSite };
            var textPage = new BasicMappedWebpage { Name = "Test Page", Parent = parent, UrlSegment = "parent/test-page", Site = CurrentSite };
            var existingPage = new BasicMappedWebpage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page",
                                       Parent = textPage,
                                       Site = CurrentSite
                                   };
            var existingPage2 = new BasicMappedWebpage
                                   {
                                       Name = "Nested Page",
                                       UrlSegment = "parent/test-page/nested-page-1",
                                       Parent = textPage,
                                       Site = CurrentSite
                                   };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(parent);
                session.SaveOrUpdate(textPage);
                session.SaveOrUpdate(existingPage);
                session.SaveOrUpdate(existingPage2);
            });

            var documentUrl = documentService.GetDocumentUrl("Nested Page", textPage, true);

            documentUrl.Should().Be("parent/test-page/nested-page-2");
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
            var textPage = new BasicMappedWebpage();

            documentService.SetTags(null, textPage);

            textPage.Tags.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentService_SetTags_IfTagsHasOneStringTheTagListShouldHave1Tag()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test tag", textPage);

            textPage.Tags.Should().HaveCount(1);
        }

        [Fact]
        public void DocumentService_SetTags_IfTagsHasTwoCommaSeparatedTagsTheTagListShouldHave2Tags()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test 1, test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldTrimTagNames()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test 1, test 2", textPage);

            textPage.Tags[1].Name.Should().Be("test 2");
        }

        [Fact]
        public void DocumentService_SetTags_ShouldSaveGeneratedTags()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test 1, test 2", textPage);

            Session.QueryOver<Tag>().RowCount().Should().Be(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldNotRecreateTags()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();
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
            var textPage = new BasicMappedWebpage();
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
            var textPage = new BasicMappedWebpage();
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
            var textPage = new BasicMappedWebpage();

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
            var textPage = new BasicMappedWebpage();
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
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test 1,,test 2", textPage);

            textPage.Tags.Should().HaveCount(2);
            Session.QueryOver<Tag>().List().Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetTags_ShouldNotCreateTagsWithEmptyNamesForTrailingComma()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            documentService.SetTags("test 1, test 2, ", textPage);

            textPage.Tags.Should().HaveCount(2);
            Session.QueryOver<Tag>().List().Should().HaveCount(2);
        }

        [Fact]
        public void DocumentService_SetOrder_ShouldSetTheDocumentOrderOfTheDocumentWithTheSetId()
        {
            var documentService = GetDocumentService();
            var textPage = new BasicMappedWebpage();

            Session.Transact(session => session.SaveOrUpdate(textPage));

            documentService.SetOrder(textPage.Id, 2);

            textPage.DisplayOrder.Should().Be(2);
        }

        [Fact]
        public void DocumentService_SearchDocuments_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(doc1);
                session.SaveOrUpdate(doc2);
            });
            var documentService = GetDocumentService();

            var searchResultModels = documentService.SearchDocuments<BasicMappedWebpage>("Test");

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        [Fact]
        public void DocumentService_SearchDocumentsDetailed_ReturnsAnIEnumerableOfSearchResultModelsWhereTheNameMatches()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            Session.Transact(session =>
            {
                session.SaveOrUpdate(doc1);
                session.SaveOrUpdate(doc2);
            });
            var documentService = GetDocumentService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", null);

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        [Fact]
        public void DocumentService_SearchDocumentsDetailed_FiltersByParentIfIdIsSet()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            var doc3 = new BasicMappedWebpage { Name = "Another Name" };
            Session.Transact(session =>
                                 {
                                     doc1.Parent = doc2;
                                     session.SaveOrUpdate(doc1);
                                     session.SaveOrUpdate(doc2);
                                     session.SaveOrUpdate(doc3);
                                 });
            var documentService = GetDocumentService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc3.Id);

            searchResultModels.Should().HaveCount(0);
        }

        [Fact]
        public void DocumentService_SearchDocumentsDetailed_FiltersByParentIfIdIsSetReturnsIfItIsCorrect()
        {
            var doc1 = new BasicMappedWebpage { Name = "Test" };
            var doc2 = new BasicMappedWebpage { Name = "Different Name" };
            var doc3 = new BasicMappedWebpage { Name = "Another Name" };
            Session.Transact(session =>
                                 {
                                     doc1.Parent = doc2;
                                     session.SaveOrUpdate(doc1);
                                     session.SaveOrUpdate(doc2);
                                     session.SaveOrUpdate(doc3);
                                 });
            var documentService = GetDocumentService();

            var searchResultModels = documentService.SearchDocumentsDetailed<BasicMappedWebpage>("Test", doc2.Id);

            searchResultModels.Should().HaveCount(1);
            searchResultModels.First().Name.Should().Be("Test");
        }

        [Fact]
        public void DocumentService_AnyWebpages_ReturnsFalseWhenNoWebpagesAreSaved()
        {
            var documentService = GetDocumentService();

            documentService.AnyWebpages().Should().BeFalse();
        }

        [Fact]
        public void DocumentService_AnyWebpages_ReturnsTrueOnceAWebpageIsAdded()
        {
            var documentService = GetDocumentService();

            documentService.AddDocument(new BasicMappedWebpage { Site = CurrentSite });

            documentService.AnyWebpages().Should().BeTrue();
        }

        [Fact]
        public void DocumentService_AnyPublishedWebpages_ReturnsFalseWhenThereAreNoWebpages()
        {
            var documentService = GetDocumentService();

            documentService.AnyPublishedWebpages().Should().BeFalse();
        }

        [Fact]
        public void DocumentService_AnyPublishedWebpages_ReturnsFalseWhenThereAreWebpagesButTheyAreNotPublished()
        {
            var documentService = GetDocumentService();

            documentService.AddDocument(new BasicMappedWebpage() { Site = CurrentSite });

            documentService.AnyPublishedWebpages().Should().BeFalse();
        }

        [Fact]
        public void DocumentService_AnyPublishedWebpages_ReturnsTrueOnceAPublishedWebpageIsAdded()
        {
            var documentService = GetDocumentService();

            documentService.AddDocument(new BasicMappedWebpage { Site = CurrentSite, PublishOn = DateTime.UtcNow.AddDays(-1) });

            documentService.AnyPublishedWebpages().Should().BeTrue();
        }

        [Fact]
        public void DocumentService_HideWidget_AddsAWidgetToTheHiddenWidgetsListIfItIsNotInTheShownList()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textPage = new BasicMappedWebpage { ShownWidgets = new List<Widget>(), HiddenWidgets = new List<Widget>() };
            documentService.SaveDocument(textPage);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            documentService.HideWidget(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void DocumentService_HideWidget_RemovesAWidgetFromTheShownListIfItIsIncluded()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            var textPage = new BasicMappedWebpage
            {
                ShownWidgets = new List<Widget> { textWidget },
                HiddenWidgets = new List<Widget>()
            };
            documentService.SaveDocument(textPage);

            documentService.HideWidget(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().NotContain(textWidget);
        }

        [Fact]
        public void DocumentService_HideWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            var textPage = new BasicMappedWebpage
            {
                ShownWidgets = new List<Widget> { textWidget },
                HiddenWidgets = new List<Widget>()
            };
            documentService.SaveDocument(textPage);

            documentService.HideWidget(textPage, -1);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }


        [Fact]
        public void DocumentService_ShowWidget_AddsAWidgetToTheShownWidgetsListIfItIsNotInTheHiddenList()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textPage = new BasicMappedWebpage { ShownWidgets = new List<Widget>(), HiddenWidgets = new List<Widget>() };
            documentService.SaveDocument(textPage);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            documentService.ShowWidget(textPage, textWidget.Id);

            textPage.ShownWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void DocumentService_ShowWidget_RemovesAWidgetFromTheHiddenListIfItIsIncluded()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            var textPage = new BasicMappedWebpage
            {
                ShownWidgets = new List<Widget>(),
                HiddenWidgets = new List<Widget> { textWidget }
            };
            documentService.SaveDocument(textPage);

            documentService.ShowWidget(textPage, textWidget.Id);

            textPage.HiddenWidgets.Should().NotContain(textWidget);
        }

        [Fact]
        public void DocumentService_ShowWidget_DoesNothingIfTheWidgetIdIsInvalid()
        {
            var documentService = GetDocumentService();
            var widgetService = new WidgetService(Session);

            var textWidget = new BasicMappedWidget();
            widgetService.SaveWidget(textWidget);

            var textPage = new BasicMappedWebpage
            {
                ShownWidgets = new List<Widget>(),
                HiddenWidgets = new List<Widget> { textWidget }
            };
            documentService.SaveDocument(textPage);

            documentService.ShowWidget(textPage, -1);

            textPage.HiddenWidgets.Should().Contain(textWidget);
        }

        [Fact]
        public void DocumentService_PublishNow_UnpublishedWebpageWillGetPublishedOnValue()
        {
            var documentService = GetDocumentService();

            var textPage = new BasicMappedWebpage();

            Session.Transact(session => session.Save(textPage));

            documentService.PublishNow(textPage);

            textPage.PublishOn.Should().HaveValue();
        }

        [Fact]
        public void DocumentService_PublishNow_PublishedWebpageShouldNotChangeValue()
        {
            var documentService = GetDocumentService();

            var publishOn = DateTime.Now.AddDays(-1);
            var textPage = new BasicMappedWebpage { PublishOn = publishOn };

            Session.Transact(session => session.Save(textPage));

            documentService.PublishNow(textPage);

            textPage.PublishOn.Should().Be(publishOn);
        }


        [Fact]
        public void DocumentService_Unpublish_ShouldSetPublishOnToNull()
        {
            var documentService = GetDocumentService();

            var publishOn = DateTime.Now.AddDays(-1);
            var textPage = new BasicMappedWebpage { PublishOn = publishOn };

            Session.Transact(session => session.Save(textPage));

            documentService.Unpublish(textPage);

            textPage.PublishOn.Should().NotHaveValue();
        }

        [Fact]
        public void DocumentService_DeleteDocument_ShouldCallSessionDelete()
        {
            var session = A.Fake<ISession>();
            var documentService = GetDocumentService(session);

            var textPage = new BasicMappedWebpage();

            documentService.DeleteDocument(textPage);

            A.CallTo(() => session.Delete(textPage)).MustHaveHappened();
        }

        [Fact]
        public void DocumentService_GetDocumentVersion_CallsSessionGetDocumentVersionWithSpecifiedId()
        {
            var session = A.Fake<ISession>();
            var documentService = GetDocumentService(session);

            documentService.GetDocumentVersion(1);

            A.CallTo(() => session.Get<DocumentVersion>(1)).MustHaveHappened();
        }

        [Fact]
        public void DocumentService_GetDocumentVersion_ReturnsResultOfCallToSessionGet()
        {
            var session = A.Fake<ISession>();
            var documentVersion = new DocumentVersion();
            A.CallTo(() => session.Get<DocumentVersion>(1)).Returns(documentVersion);
            var documentService = GetDocumentService(session);

            var version = documentService.GetDocumentVersion(1);
            version.Should().Be(documentVersion);
        }

        [Fact]
        public void DocumentService_AddDocument_RootDocShouldSetDisplayOrderToMaxOfNonParentDocsPlus1()
        {
            for (int i = 0; i < 4; i++)
            {
                Session.Transact(session => session.Save(new StubDocument { DisplayOrder = i, Site = CurrentSite }));
            }

            var documentService = GetDocumentService();

            var stubDocument = new StubDocument { Site = CurrentSite };
            documentService.AddDocument(stubDocument);

            stubDocument.DisplayOrder.Should().Be(4);
        }

    }
}
