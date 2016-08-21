using System.Linq;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class UrlHistoryAdminServiceTests 
    {
        private readonly UrlHistoryAdminService _urlHistoryAdminService;
        private InMemoryRepository<UrlHistory> _urlHistoryRepository = new InMemoryRepository<UrlHistory>();

        public UrlHistoryAdminServiceTests()
        {
            _urlHistoryAdminService = new UrlHistoryAdminService(_urlHistoryRepository);
        }

        [Fact]
        public void UrlHistoryAdminService_Add_AddsAHistoryToTheDb()
        {
            _urlHistoryAdminService.Add(new UrlHistory {Webpage = new StubWebpage()});

            _urlHistoryRepository.Query().Count().Should().Be(1);
        }

        [Fact]
        public void UrlHistoryAdminService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            _urlHistoryRepository.Add(urlHistory);

            _urlHistoryAdminService.Delete(urlHistory);

            _urlHistoryRepository.Query().ToList().Should().NotContain(urlHistory);
        }
    }
}