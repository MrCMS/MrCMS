using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Web.Apps.Admin.Tests.Stubs;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Services
{
    public class UrlHistoryAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly UrlHistoryAdminService _urlHistoryAdminService;
        private readonly Site _site;
        private readonly IMapper _mapper;

        public UrlHistoryAdminServiceTests()
        {
            _site = new Site();
            _mapper = A.Fake<IMapper>();
            _urlHistoryAdminService = new UrlHistoryAdminService(Session,_site, _mapper);
        }

        [Fact]
        public void UrlHistoryAdminService_Add_AddsAHistoryToTheDb()
        {
            var webpage = new StubWebpage();
            var addUrlHistoryModel = new AddUrlHistoryModel {WebpageId = webpage.Id};
            var urlHistory = new UrlHistory();
            A.CallTo(() => _mapper.Map<UrlHistory>(addUrlHistoryModel)).Returns(urlHistory);

            Session.Transact(session => session.Save(webpage));

            _urlHistoryAdminService.Add(
                addUrlHistoryModel
            );
            Session.QueryOver<UrlHistory>().RowCount().Should().Be(1);
        }

        [Fact]
        public void UrlHistoryAdminService_Delete_ShouldRemoveAPassedHistoryFromTheDb()
        {
            var urlHistory = new UrlHistory();
            Session.Transact(session => session.Save(urlHistory));

            _urlHistoryAdminService.Delete(urlHistory.Id);

            Session.QueryOver<UrlHistory>().List().Should().NotContain(urlHistory);
        }
    }
}