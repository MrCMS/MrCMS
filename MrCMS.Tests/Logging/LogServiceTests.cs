using System.Collections.Generic;
using System.Linq;
using Elmah;
using FluentAssertions;
using MrCMS.Logging;
using MrCMS.Helpers;
using MrCMS.Settings;
using Xunit;

namespace MrCMS.Tests.Logging
{
    public class LogServiceTests : InMemoryDatabaseTest
    {
        private readonly LogService _logService;
        private readonly SiteSettings _siteSettings;

        public LogServiceTests()
        {
            _siteSettings = new SiteSettings();
            _logService = new LogService(Session, _siteSettings);
        }

        [Fact]
        public void LogService_GetAllLogEntries_ReturnsAllLogEntries()
        {
            var list = CreateLogList();

            var logs = _logService.GetAllLogEntries();

            logs.Should().BeEquivalentTo(list);
        }

        [Fact]
        public void LogService_DeleteAllLogs_ShouldRemoveAllLogs()
        {
            var list = CreateLogList();

            _logService.DeleteAllLogs();

            Session.QueryOver<Log>().RowCount().Should().Be(0);
        }

        [Fact]
        public void LogService_DeleteLog_ShouldRemoveTheDeletedLog()
        {
            var list = CreateLogList();

            _logService.DeleteLog(list[0]);

            Session.QueryOver<Log>().List().Should().NotContain(list[0]);
        }


        private static List<Log> CreateLogList()
        {
            var logList = Enumerable.Range(1, 20).Select(i => new Log { Message = i.ToString(), Error = new Error() }).ToList();
            logList.ForEach(log => Session.Transact(session => session.Save(log)));
            return logList;
        }
    }
}