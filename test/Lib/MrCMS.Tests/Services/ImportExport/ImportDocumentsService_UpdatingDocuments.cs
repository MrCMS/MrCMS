using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services.ImportExport
{
    //public class ImportDocumentsService_UpdatingDocuments : InMemoryDatabaseTest
    //{
    //    private readonly ImportDocumentsService _importDocumentsService;
    //    private readonly IIndexService _indexService;
    //    private readonly IUpdateTagsService _updateTagsService;
    //    private readonly IUpdateUrlHistoryService _updateUrlHistoryService;

    //    public ImportDocumentsService_UpdatingDocuments()
    //    {
    //        _indexService = A.Fake<IIndexService>();
    //        _updateTagsService = A.Fake<IUpdateTagsService>();
    //        _updateUrlHistoryService = A.Fake<IUpdateUrlHistoryService>();
    //        _importDocumentsService = new ImportDocumentsService(Session, CurrentSite, _indexService, _updateTagsService, _updateUrlHistoryService);
    //    }

    //    [Fact]
    //    public void ShouldNotReAddAnExistingDocument()
    //    {
    //        Session.Transact(session => session.Save(new BasicMappedWebpage { UrlSegment = "test" }));
    //        Session.QueryOver<Webpage>().RowCount().Should().Be(1);

    //        _importDocumentsService.ImportDocumentsFromDTOs(new HashSet<DocumentImportDTO>
    //                                                            {
    //                                                                new DocumentImportDTO{UrlSegment = "test"}
    //                                                            });

    //        Session.QueryOver<Webpage>().RowCount().Should().Be(1);
    //    }

    //    [Fact]
    //    public void ShouldUpdatePropertiesFromDTO()
    //    {
    //        Session.Transact(session => session.Save(new BasicMappedWebpage { UrlSegment = "test", Name = "old" }));
    //        Session.QueryOver<Webpage>().RowCount().Should().Be(1);

    //        _importDocumentsService.ImportDocumentsFromDTOs(new HashSet<DocumentImportDTO>
    //                                                            {
    //                                                                new DocumentImportDTO{UrlSegment = "test", Name = "New"}
    //                                                            });

    //        Session.QueryOver<Webpage>().List()[0].Name.Should().Be("New");
    //    }

    //    [Fact]
    //    public void ShouldCallSetTags()
    //    {
    //        var dto = new DocumentImportDTO { UrlSegment = "test", Name = "New", DocumentType = typeof(BasicMappedWebpage).Name };

    //        _importDocumentsService.ImportDocumentsFromDTOs(new HashSet<DocumentImportDTO> { dto });

    //        A.CallTo(() => _updateTagsService.SetTags(dto, A<Webpage>._)).MustHaveHappened();
    //    }

    //    [Fact]
    //    public void ShouldCallSetUrlHistories()
    //    {
    //        var dto = new DocumentImportDTO { UrlSegment = "test", Name = "New", DocumentType = typeof(BasicMappedWebpage).Name };

    //        _importDocumentsService.ImportDocumentsFromDTOs(new HashSet<DocumentImportDTO> { dto });

    //        A.CallTo(() => _updateUrlHistoryService.SetUrlHistory(dto, A<Webpage>._)).MustHaveHappened();
    //    }
    //}
}