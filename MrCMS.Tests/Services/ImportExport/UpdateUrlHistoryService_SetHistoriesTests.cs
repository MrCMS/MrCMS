using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using NHibernate.Linq;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateUrlHistoryService_SetHistoriesTests : InMemoryDatabaseTest
    {
        private readonly UpdateUrlHistoryService _updateUrlHistoryService;

        public UpdateUrlHistoryService_SetHistoriesTests()
        {
            _updateUrlHistoryService = new UpdateUrlHistoryService(Session);
        }

        [Fact]
        public void AddsANewUrlHistory()
        {
            GetAllHistories().Should().HaveCount(0);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                new BasicMappedWebpage());

            GetAllHistories().Should().HaveCount(1);
            GetAllHistories().ElementAt(0).UrlSegment.Should().Be("test");
        }

        [Fact]
        public void AssignsAddedUrlHistoryToTheWebpage()
        {
            GetAllHistories().Should().HaveCount(0);
            var basicMappedWebpage = new BasicMappedWebpage();

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage);

            GetAllHistories().ElementAt(0).Webpage.Should().Be(basicMappedWebpage);
        }

        [Fact]
        public void UnAssigningAUrlHistoryShouldSetTheWebpageToNull()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage;
            Session.Transact(session => session.Save(urlHistory));

            GetAllHistories().Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage);

            GetAllHistories().ElementAt(0).Webpage.Should().BeNull();
        }

        [Fact]
        public void UnAssigningAUrlHistoryRemoveTheItemFromTheWebpageUrlList()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage;
            Session.Transact(session => session.Save(urlHistory));

            basicMappedWebpage.Urls.Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage);

            basicMappedWebpage.Urls.Should().HaveCount(0);
        }

        [Fact]
        public void MovesTheUrlHistoryBetweenPagesIfTheyAreChanged()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage1 = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage {Urls = new List<UrlHistory>()};
            Session.Transact(session => session.Save(urlHistory));
            Session.Transact(session => session.Save(basicMappedWebpage1));
            Session.Transact(session => session.Save(basicMappedWebpage2));

            basicMappedWebpage1.Urls.Should().HaveCount(1);
            basicMappedWebpage2.Urls.Should().HaveCount(0);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage1);
            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage2);

            basicMappedWebpage1.Urls.Should().HaveCount(0);
            basicMappedWebpage2.Urls.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldNotCreateNewUrlHistoryWhileMovingUrls()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage1 = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage {Urls = new List<UrlHistory>()};
            Session.Transact(session => session.Save(urlHistory));
            Session.Transact(session => session.Save(basicMappedWebpage1));
            Session.Transact(session => session.Save(basicMappedWebpage2));

            GetAllHistories().Should().HaveCount(1);

            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage1);
            _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage2);

            GetAllHistories().Should().HaveCount(1);
        }

        private IEnumerable<UrlHistory> GetAllHistories()
        {
            return Session.Query<UrlHistory>();
        }
    }
}