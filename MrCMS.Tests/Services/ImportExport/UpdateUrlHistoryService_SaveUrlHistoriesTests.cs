using FluentAssertions;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services.ImportExport;
using Xunit;

namespace MrCMS.Tests.Services.ImportExport
{
    public class UpdateUrlHistoryService_SaveUrlHistoriesTests : InMemoryDatabaseTest
    {
        private readonly UpdateUrlHistoryService _updateUrlHistoryService;

        public UpdateUrlHistoryService_SaveUrlHistoriesTests()
        {
            _updateUrlHistoryService = new UpdateUrlHistoryService(Session, CurrentSite);
            _updateUrlHistoryService.Initialise();
        }

        [Fact]
        public void AnyUnassignedUrlHistoriesAreDeleted()
        {
            Session.Transact(session => session.Save(new UrlHistory { UrlSegment = "test", Webpage = null }));
            _updateUrlHistoryService.Initialise();
            _updateUrlHistoryService.UrlHistories.Should().HaveCount(1);
            Session.QueryOver<UrlHistory>().RowCount().Should().Be(1);

            _updateUrlHistoryService.SaveUrlHistories();

            Session.QueryOver<UrlHistory>().RowCount().Should().Be(0);
        }
    }
}