using System;
using System.Collections.Generic;
using FakeItEasy;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Web.Apps.Core.Pages;
using NHibernate;
using Xunit;
using MrCMS.Services.ImportExport.DTOs;
using FluentAssertions;
namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsServiceTests
    {
        private readonly IDocumentService _documentService;
        private readonly ITagService _tagService;
        private readonly ImportDocumentsService _importDocumentsService;
         private readonly IUrlHistoryService _urlHistoryService;
        private ISession _session;

        public ImportDocumentsServiceTests()
        {
            _documentService = A.Fake<IDocumentService>();
            _tagService = A.Fake<ITagService>();
            _urlHistoryService = A.Fake<IUrlHistoryService>();
            _session = A.Fake<ISession>();

            _importDocumentsService = new ImportDocumentsService(_documentService,_tagService,_urlHistoryService, _session);
        }

        [Fact(Skip = "To be refactored")]
        public void ImportDocumentsService_ImportDocumentsFromDTOs_ShouldCallGetAllDocuments()
        {
            var documentDto = new DocumentImportDataTransferObject()
            {
                UrlSegment = "test-url",
                DocumentType = "TextPage"
            };

            _importDocumentsService.ImportDocumentsFromDTOs(new List<DocumentImportDataTransferObject>(){documentDto});

            A.CallTo(() => _documentService.GetAllDocuments<Document>()).MustHaveHappened();
        }

        [Fact(Skip = "To be refactored")]
        public void ImportDocumentsService_ImportDocument_ShouldSetDocumentPrimaryProperties()
        {
            var currentTime = DateTime.UtcNow;
            var documentDTO = new DocumentImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Name = "Test Document",
                ParentUrl = "test-parent-url",
                BodyContent = "Test Body Content",
                MetaDescription = "Test SEO Description",
                MetaKeywords = "Test, Thought",
                MetaTitle = "Test SEO Title",
                DisplayOrder = 0,
                RevealInNavigation = true,
                RequireSSL = false,
                PublishDate = currentTime,
                DocumentType = "TextPage",
                Tags = new List<string>(){"Test"}
            };

          
            var result = _importDocumentsService.ImportDocument(documentDTO);

            result.UrlSegment.Should().BeEquivalentTo("test-url");
            result.Name.Should().BeEquivalentTo("Test Document");
            result.BodyContent.Should().BeEquivalentTo("Test Body Content");
            result.MetaDescription.Should().BeEquivalentTo("Test SEO Description");
            result.MetaKeywords.Should().BeEquivalentTo("Test, Thought");
            result.MetaTitle.Should().BeEquivalentTo("Test SEO Title");
            result.DisplayOrder.Should().Be(0);
            result.RevealInNavigation.Should().BeTrue();
            result.RequiresSSL.Should().BeFalse();
            result.PublishOn.Should().Be(currentTime);
            result.DocumentType.Should().BeEquivalentTo("TextPage");
            result.Tags.Should().HaveCount(1);
        }

        [Fact(Skip = "To be refactored")]
        public void ImportDocumentsService_ImportDocument_ShouldCallGetByUrlSegmentOfUrlHistoryService()
        {
            var currentTime = DateTime.UtcNow;
            var documentDTO = new DocumentImportDataTransferObject()
            {
                UrlSegment = "test-url",
                Name = "Test Document",
                ParentUrl = "test-parent-url",
                BodyContent = "Test Body Content",
                MetaDescription = "Test SEO Description",
                MetaKeywords = "Test, Thought",
                MetaTitle = "Test SEO Title",
                DisplayOrder = 2,
                RevealInNavigation = true,
                RequireSSL = false,
                PublishDate = currentTime,
                DocumentType = "Article",
                UrlHistory = new List<string>() { "test-url-old" },
                Tags = new List<string>() { "Test" }
            };


            var document = new TextPage { Name = "Test Document", UrlSegment = "test-url" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Webpage>(documentDTO.UrlSegment)).Returns(document);

            var parent = new TextPage { Name = "Test Parent", UrlSegment = "test-parent-url" };
            A.CallTo(() => _documentService.GetDocumentByUrl<Webpage>(documentDTO.ParentUrl)).Returns(parent);

            var urlHistory = new UrlHistory() { UrlSegment = "test-url-old", Webpage = document };
            A.CallTo(() => _urlHistoryService.GetByUrlSegment("test-url-old")).Returns(urlHistory);

            var result = _importDocumentsService.ImportDocument(documentDTO);

            A.CallTo(() => _urlHistoryService.GetByUrlSegment("test-url-old")).MustHaveHappened();
        }
    }
}