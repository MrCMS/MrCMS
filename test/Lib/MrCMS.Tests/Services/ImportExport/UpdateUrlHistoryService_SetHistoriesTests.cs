using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport;
using MrCMS.Services.ImportExport.DTOs;
using MrCMS.Tests.Stubs;
using MrCMS.TestSupport;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateUrlHistoryService_SetHistoriesTests
    {
        private readonly UpdateUrlHistoryService _updateUrlHistoryService;
        private readonly InMemoryRepository<UrlHistory> _urlHistoryRepository;

        public UpdateUrlHistoryService_SetHistoriesTests()
        {
            _urlHistoryRepository = new InMemoryRepository<UrlHistory>();
            _updateUrlHistoryService = new UpdateUrlHistoryService(_urlHistoryRepository);
        }

        [Fact]
        public async Task AddsANewUrlHistory()
        {
            GetAllHistories().Should().HaveCount(0);

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                new BasicMappedWebpage());

            GetAllHistories().Should().HaveCount(1);
            GetAllHistories().ElementAt(0).UrlSegment.Should().Be("test");
        }

        [Fact]
        public async Task AssignsAddedUrlHistoryToTheWebpage()
        {
            GetAllHistories().Should().HaveCount(0);
            var basicMappedWebpage = new BasicMappedWebpage();

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage);

            GetAllHistories().ElementAt(0).Webpage.Should().Be(basicMappedWebpage);
        }

        [Fact]
        public async Task UnAssigningAUrlHistoryShouldSetTheWebpageToNull()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage;
            await _urlHistoryRepository.Add(urlHistory);

            GetAllHistories().Should().HaveCount(1);

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage);

            GetAllHistories().ElementAt(0).Webpage.Should().BeNull();
        }

        [Fact]
        public async Task UnAssigningAUrlHistoryRemoveTheItemFromTheWebpageUrlList()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage;
            await _urlHistoryRepository.Add(urlHistory);

            basicMappedWebpage.Urls.Should().HaveCount(1);

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage);

            basicMappedWebpage.Urls.Should().HaveCount(0);
        }

        [Fact]
        public async Task MovesTheUrlHistoryBetweenPagesIfTheyAreChanged()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage1 = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage {Urls = new List<UrlHistory>()};
            await _urlHistoryRepository.Add(urlHistory);

            basicMappedWebpage1.Urls.Should().HaveCount(1);
            basicMappedWebpage2.Urls.Should().HaveCount(0);

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage1);
            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage2);

            basicMappedWebpage1.Urls.Should().HaveCount(0);
            basicMappedWebpage2.Urls.Should().HaveCount(1);
        }

        [Fact]
        public async Task ShouldNotCreateNewUrlHistoryWhileMovingUrls()
        {
            var urlHistory = new UrlHistory {UrlSegment = "test"};
            var basicMappedWebpage1 = new BasicMappedWebpage {Urls = new List<UrlHistory> {urlHistory}};
            urlHistory.Webpage = basicMappedWebpage1;
            var basicMappedWebpage2 = new BasicMappedWebpage {Urls = new List<UrlHistory>()};
            await _urlHistoryRepository.Add(urlHistory);

            GetAllHistories().Should().HaveCount(1);

            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string>()},
                basicMappedWebpage1);
            await _updateUrlHistoryService.SetUrlHistory(new DocumentImportDTO {UrlHistory = new List<string> {"test"}},
                basicMappedWebpage2);

            GetAllHistories().Should().HaveCount(1);
        }

        private IEnumerable<UrlHistory> GetAllHistories()
        {
            return _urlHistoryRepository.Query();
        }
    }
}