using System.Collections.Generic;
using System.Linq;
using Elmah;
using FluentAssertions;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Web.Tests.TestSupport;
using Xunit;

namespace MrCMS.Web.Tests.Areas.Admin.Services
{
    public class LogAdminServiceTests 
    {
        private readonly LogAdminService _logService;
        private InMemoryRepository<Log> _logRepository = new InMemoryRepository<Log>();
        private InMemoryRepository<Site> _siteRepository;

        public LogAdminServiceTests()
        {
            _siteRepository = new InMemoryRepository<Site>();
            _logService = new LogAdminService(_logRepository, _siteRepository);
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

            _logRepository.Query().Count().Should().Be(0);
        }

        [Fact]
        public void LogAdminService_DeleteLog_ShouldRemoveTheDeletedLog()
        {
            List<Log> list = CreateLogList();

            _logService.DeleteLog(list[0]);

            _logRepository.Query().Should().NotContain(list[0]);
        }


        private List<Log> CreateLogList()
        {
            List<Log> logList =
                Enumerable.Range(1, 20).Select(i => new Log {Message = i.ToString(), Error = new Error()}).ToList();
            logList.ForEach(log => _logRepository.Add(log));
            return logList;
        }
    }
}