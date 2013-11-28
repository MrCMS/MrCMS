using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class ImportDocumentsService_AddingDocuments : InMemoryDatabaseTest
    {
        private readonly ImportDocumentsService _importDocumentsService;
        private readonly IIndexService _indexService;
        private readonly IUpdateTagsService _updateTagsService;
        private readonly IUpdateUrlHistoryService _updateUrlHistoryService;

        public ImportDocumentsService_AddingDocuments()
        {
            _indexService = A.Fake<IIndexService>();
            _updateTagsService = A.Fake<IUpdateTagsService>();
            _updateUrlHistoryService = A.Fake<IUpdateUrlHistoryService>();
            _importDocumentsService = new ImportDocumentsService(Session, CurrentSite, _indexService, _updateTagsService, _updateUrlHistoryService);
        }

        [Fact]
        public void ANewWebpageShouldBeAddedToTheSession()
        {
            _importDocumentsService.ImportDocumentsFromDTOs(new List<DocumentImportDTO>
                                                                {
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Test",
                                                                            UrlSegment = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        }
                                                                });

            var webpages = Session.QueryOver<Webpage>().List();
            webpages.Should().HaveCount(1);
            webpages[0].Should().BeOfType<BasicMappedWebpage>();
            webpages[0].Name.Should().Be("Test");
            webpages[0].UrlSegment.Should().Be("test");
        }

        [Fact]
        public void ShouldBeAbleToAddAChildPage()
        {
            _importDocumentsService.ImportDocumentsFromDTOs(new List<DocumentImportDTO>
                                                                {
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Test",
                                                                            UrlSegment = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        },
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Child",
                                                                            UrlSegment = "child",
                                                                            ParentUrl = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        }
                                                                });

            var webpages = Session.QueryOver<Webpage>().List();
            webpages.Should().HaveCount(2);
            webpages.Where(webpage => webpage.Parent == null).Should().HaveCount(1);
            webpages.First(webpage => webpage.Parent != null).Parent.UrlSegment.Should().Be("test");
        }

        [Fact]
        public void ShouldBeAbleToAddAChildPageRegardlessOfImportOrder()
        {
            _importDocumentsService.ImportDocumentsFromDTOs(new List<DocumentImportDTO>
                                                                {
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Child",
                                                                            UrlSegment = "child",
                                                                            ParentUrl = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        },
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Test",
                                                                            UrlSegment = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        }
                                                                });

            var webpages = Session.QueryOver<Webpage>().List();
            webpages.Should().HaveCount(2);
            webpages.Where(webpage => webpage.Parent == null).Should().HaveCount(1);
            webpages.First(webpage => webpage.Parent != null).Parent.UrlSegment.Should().Be("test");
        }

        [Fact]
        public void ShouldBeAbleToAddAChildPageRegardlessOfImportOrderWithNoneHierarchicalUrls()
        {
            _importDocumentsService.ImportDocumentsFromDTOs(new List<DocumentImportDTO>
                                                                {
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Another Child",
                                                                            UrlSegment = "another-child",
                                                                            ParentUrl = "child",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        },
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Child",
                                                                            UrlSegment = "child",
                                                                            ParentUrl = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        },
                                                                    new DocumentImportDTO
                                                                        {
                                                                            Name = "Test",
                                                                            UrlSegment = "test",
                                                                            DocumentType =
                                                                                typeof (BasicMappedWebpage).Name
                                                                        }
                                                                });

            var webpages = Session.QueryOver<Webpage>().List();
            webpages.Should().HaveCount(3);
            webpages.Where(webpage => webpage.Parent == null).Should().HaveCount(1);
            var children = webpages.Where(webpage => webpage.Parent != null).ToList();
            children[0].Parent.UrlSegment.Should().Be("test");
            children[1].Parent.UrlSegment.Should().Be("child");
        }
    }
}