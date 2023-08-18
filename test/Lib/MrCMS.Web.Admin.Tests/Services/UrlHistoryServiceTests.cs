using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Mapping;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class UrlHistoryAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly UrlHistoryAdminService _urlHistoryAdminService;
        private readonly ISessionAwareMapper _mapper;

        public UrlHistoryAdminServiceTests()
        {
            _mapper = A.Fake<ISessionAwareMapper>();
            _urlHistoryAdminService = new UrlHistoryAdminService(Session,_mapper);
        }

        [Fact]
        public async Task UrlHistoryAdminService_Add_AddsAHistoryToTheDb()
        {
            var webpage = new StubWebpage();
            var addUrlHistoryModel = new AddUrlHistoryModel {WebpageId = webpage.Id};
            var urlHistory = new UrlHistory();
            A.CallTo(() => _mapper.Map<UrlHistory>(addUrlHistoryModel)).Returns(urlHistory);

            await Session.TransactAsync(session => session.SaveAsync(webpage));

            await _urlHistoryAdminService.Add(
                addUrlHistoryModel
            );
            (await Session.QueryOver<UrlHistory>().RowCountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task UrlHistoryAdminService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            await Session.TransactAsync(session => session.SaveAsync(urlHistory));

            await _urlHistoryAdminService.Delete(urlHistory.Id);

            (await Session.QueryOver<UrlHistory>().ListAsync()).Should().NotContain(urlHistory);
        }
    }
}