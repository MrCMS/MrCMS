using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class UrlHistoryAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly UrlHistoryAdminService _urlHistoryAdminService;

        public UrlHistoryAdminServiceTests()
        {
            _urlHistoryAdminService = new UrlHistoryAdminService(Session);
        }

        [Fact]
        public void UrlHistoryAdminService_Add_AddsAHistoryToTheDb()
        {
            _urlHistoryAdminService.Add(new UrlHistory {Webpage = new StubWebpage()});

            Session.QueryOver<UrlHistory>().RowCount().Should().Be(1);
        }

        [Fact]
        public void UrlHistoryAdminService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            Session.Transact(session => session.Save(urlHistory));

            _urlHistoryAdminService.Delete(urlHistory);

            Session.QueryOver<UrlHistory>().List().Should().NotContain(urlHistory);
        }
    }
}