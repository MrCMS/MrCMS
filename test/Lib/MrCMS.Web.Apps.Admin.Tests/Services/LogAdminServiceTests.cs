using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.TestSupport;
using MrCMS.Web.Apps.Admin.Services;
using Xunit;

namespace MrCMS.Web.Apps.Admin.Tests.Services
{
    public class LogAdminServiceTests : MrCMSTest
    {
        private readonly LogAdminService _logService;
        private readonly IGlobalRepository<Log> _logRepository = A.Fake<IGlobalRepository<Log>>();
        private readonly IGlobalRepository<Site> _siteRepository = A.Fake<IGlobalRepository<Site>>();

        public LogAdminServiceTests()
        {
            _logService = new LogAdminService(_logRepository, _siteRepository);
        }

        [Fact]
        public void LogAdminService_GetAllLogEntries_ReturnsAllLogEntries()
        {
            List<Log> list = CreateLogList();
            A.CallTo(() => _logRepository.Query()).ReturnsAsAsyncQueryable(list.ToArray());

            IList<Log> logs = _logService.GetAllLogEntries();

            logs.Should().BeEquivalentTo(list);
        }

        //[Fact]
        //public async Task LogAdminService_DeleteAllLogs_ShouldRemoveAllLogs()
        //{
        //    List<Log> list = CreateLogList();

        //    await _logService.DeleteAllLogs();

        //    Context.QueryOver<Log>().RowCount().Should().Be(0);
        //}

        //[Fact]
        //public void LogAdminService_DeleteLog_ShouldRemoveTheDeletedLog()
        //{
        //    List<Log> list = CreateLogList();

        //    _logService.DeleteLog(list[0].Id);

        //    Context.QueryOver<Log>().List().Should().NotContain(list[0]);
        //}


        private List<Log> CreateLogList()
        {
            List<Log> logList =
                Enumerable.Range(1, 20).Select(i => new Log { Message = i.ToString() }).ToList();
            //logList.ForEach(log => Context.Transact(session => session.Save(log)));
            return logList;
        }
    }
}