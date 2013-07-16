using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using Xunit;
using MrCMS.Helpers;

namespace MrCMS.Tests.Services
{
    public class UrlHistoryServiceTests:InMemoryDatabaseTest
    {
        private readonly UrlHistoryService _urlHistoryService;

        public UrlHistoryServiceTests()
        {
            _urlHistoryService = new UrlHistoryService(Session);
        }

        [Fact]
        public void UrlHistoryService_Add_AddsAHistoryToTheDb()
        {
            _urlHistoryService.Add(new UrlHistory());

            Session.QueryOver<UrlHistory>().RowCount().Should().Be(1);
        }

        [Fact]
        public void UrlHistoryService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            Session.Transact(session => session.Save(urlHistory));

            _urlHistoryService.Delete(urlHistory);

            Session.QueryOver<UrlHistory>().List().Should().NotContain(urlHistory);
        }
    }
}