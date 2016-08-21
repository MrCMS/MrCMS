using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tests.Stubs;
using MrCMS.Website;
using Xunit;

namespace MrCMS.Tests.Services
{
    //public class DocumentServiceTests 
    //{
    //    private readonly DocumentService _documentService;

    //    public DocumentServiceTests()
    //    {
    //        _documentService = new DocumentService(Session, CurrentSite);
    //    }

    //    [Fact]
    //    public void AddDocument_OnSave_AddsToRepository()
    //    {
    //        _documentService.AddDocument(new BasicMappedWebpage {Site = CurrentSite});

    //        Session.QueryOver<Document>().RowCount().Should().Be(1);
    //    }


    //    [Fact]
    //    public void GetDocument_WhenDocumentDoesNotExist_ReturnsNull()
    //    {
    //        var document = _documentService.GetDocument<BasicMappedWebpage>(1);

    //        document.Should().BeNull();
    //    }

    //    [Fact]
    //    public void DocumentService_SaveDocument_ReturnsTheSameDocument()
    //    {
    //        var document = new BasicMappedWebpage();
    //        Session.Transact(session => session.Save(document));

    //        BasicMappedWebpage updatedDocument = _documentService.SaveDocument(document);

    //        document.Should().BeSameAs(updatedDocument);
    //    }

    //    [Fact]
    //    public void DocumentService_GetAllDocuments_ShouldReturnAListOfAllDocumentsOfTheSpecifiedType()
    //    {
    //        Enumerable.Range(1, 10)
    //            .ForEach(
    //                i => Session.Transact(session => session.SaveOrUpdate(new BasicMappedWebpage {Name = "Page " + i})));

    //        IEnumerable<BasicMappedWebpage> allDocuments = _documentService.GetAllDocuments<BasicMappedWebpage>();

    //        allDocuments.Should().HaveCount(10);
    //    }

    //    [Fact]
    //    public void DocumentService_GetAllDocuments_ShouldOnlyReturnDocumentsOfSpecifiedType()
    //    {
    //        Enumerable.Range(1, 10).ForEach(i =>
    //            Session.Transact(
    //                session =>
    //                    session.SaveOrUpdate(i%2 == 0
    //                        ? (Document) new BasicMappedWebpage {Name = "Page " + i}
    //                        : new Layout {Name = "Layout " + i}
    //                        )));

    //        IEnumerable<BasicMappedWebpage> allDocuments = _documentService.GetAllDocuments<BasicMappedWebpage>();

    //        allDocuments.Should().HaveCount(5);
    //    }


    //    [Fact]
    //    public void DocumentService_GetDocumentByUrl_ReturnsTheDocumentWithTheSpecifiedUrl()
    //    {
    //        var textPage = new BasicMappedWebpage {UrlSegment = "test-page", Site = CurrentSite};
    //        Session.Transact(session => session.SaveOrUpdate(textPage));

    //        var document = _documentService.GetDocumentByUrl<BasicMappedWebpage>("test-page");

    //        document.Should().NotBeNull();
    //    }

    //    [Fact]
    //    public void DocumentService_GetDocumentByUrl_ShouldReturnNullIfTheRequestedTypeDoesNotMatch()
    //    {
    //        var site = new Site();
    //        var textPage = new BasicMappedWebpage {UrlSegment = "test-page", Site = site};
    //        Session.Transact(session => session.SaveOrUpdate(textPage));

    //        var document = _documentService.GetDocumentByUrl<Layout>("test-page");

    //        document.Should().BeNull();
    //    }

    //    [Fact]
    //    public void DocumentService_PublishNow_UnpublishedWebpageWillGetPublishedOnValue()
    //    {
    //        var textPage = new BasicMappedWebpage();

    //        Session.Transact(session => session.Save(textPage));

    //        _documentService.PublishNow(textPage);

    //        textPage.PublishOn.Should().HaveValue();
    //    }

    //    [Fact]
    //    public void DocumentService_PublishNow_PublishedWebpageShouldNotChangeValue()
    //    {
    //        DateTime publishOn = CurrentRequestData.Now.AddDays(-1);
    //        var textPage = new BasicMappedWebpage {PublishOn = publishOn};

    //        Session.Transact(session => session.Save(textPage));

    //        _documentService.PublishNow(textPage);

    //        textPage.PublishOn.Should().Be(publishOn);
    //    }


    //    [Fact]
    //    public void DocumentService_Unpublish_ShouldSetPublishOnToNull()
    //    {
    //        DateTime publishOn = CurrentRequestData.Now.AddDays(-1);
    //        var textPage = new BasicMappedWebpage {PublishOn = publishOn};

    //        Session.Transact(session => session.Save(textPage));

    //        _documentService.Unpublish(textPage);

    //        textPage.PublishOn.Should().NotHaveValue();
    //    }

    //    [Fact]
    //    public void DocumentService_DeleteDocument_ShouldCallSessionDelete()
    //    {
    //        var textPage = new BasicMappedWebpage();
    //        Session.Transact(session => session.Save(textPage));

    //        _documentService.DeleteDocument(textPage);

    //        Session.QueryOver<Webpage>().RowCount().Should().Be(0);
    //    }

    //    [Fact]
    //    public void DocumentService_AddDocument_RootDocShouldSetDisplayOrderToMaxOfNonParentDocsPlus1()
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            int order = i;
    //            Session.Transact(session => session.Save(new StubWebpage {DisplayOrder = order, Site = CurrentSite}));
    //        }

    //        var stubDocument = new StubWebpage {Site = CurrentSite};
    //        _documentService.AddDocument(stubDocument);

    //        stubDocument.DisplayOrder.Should().Be(4);
    //    }
    //}
}