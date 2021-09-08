using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.TestSupport;
using MrCMS.Web.Admin.Services;
using Xunit;

namespace MrCMS.Web.Admin.Tests.Services
{
    public class LogAdminServiceTests : InMemoryDatabaseTest
    {
        private readonly LogAdminService _logService;

        public LogAdminServiceTests()
        {
            _logService = new LogAdminService(Session);
        }


        [Fact]
        public async Task LogAdminService_DeleteAllLogs_ShouldRemoveAllLogs()
        {
            List<Log> list = await CreateLogList();

            await _logService.DeleteAllLogs();

            (await Session.QueryOver<Log>().RowCountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task LogAdminService_DeleteLog_ShouldRemoveTheDeletedLog()
        {
            List<Log> list = await CreateLogList();

            await _logService.DeleteLog(list[0].Id);

            (await Session.QueryOver<Log>().ListAsync()).Should().NotContain(list[0]);
        }


        private async Task<List<Log>> CreateLogList()
        {
            List<Log> logList =
                Enumerable.Range(1, 20).Select(i => new Log {Message = i.ToString()}).ToList();
            foreach (var log in logList)
            {
                await Session.TransactAsync(session => session.SaveAsync(log));
            }

            return logList;
        }
    }
}