using System.Collections.Generic;
using System.Linq;
using Elmah;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Web.Areas.Admin.Services;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class LogAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly LogAdminService _logService;

        public LogAdminServiceTests()
        {
            _logService = new LogAdminService(Session);
        }

        [Fact]
        public void LogAdminService_GetAllLogEntries_ReturnsAllLogEntries()
        {
            List<Log> list = CreateLogList();

            IList<Log> logs = _logService.GetAllLogEntries();

            logs.Should().BeEquivalentTo(list);
        }

        [Fact]
        public void LogAdminService_DeleteAllLogs_ShouldRemoveAllLogs()
        {
            List<Log> list = CreateLogList();

            _logService.DeleteAllLogs();

            Session.QueryOver<Log>().RowCount().Should().Be(0);
        }

        [Fact]
        public void LogAdminService_DeleteLog_ShouldRemoveTheDeletedLog()
        {
            List<Log> list = CreateLogList();

            _logService.DeleteLog(list[0]);

            Session.QueryOver<Log>().List().Should().NotContain(list[0]);
        }


        private List<Log> CreateLogList()
        {
            List<Log> logList =
                Enumerable.Range(1, 20).Select(i => new Log {Message = i.ToString(), Error = new Error()}).ToList();
            logList.ForEach(log => Session.Transact(session => session.Save(log)));
            return logList;
        }
    }
}