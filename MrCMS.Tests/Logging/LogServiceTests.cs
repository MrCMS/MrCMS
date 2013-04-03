using System.Collections.Generic;
using System.Linq;
using Elmah;
using FluentAssertions;
using MrCMS.Logging;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Tests.Logging
{
    public class LogServiceTests :InMemoryDatabaseTest
    {
        private LogService _logService;

        public LogServiceTests()
        {
            _logService= new LogService(Session);
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

        [Fact]
        public void LogService_GetAllEntriesPaged_ShouldReturn10ItemsByDefault()
        {
            var list = CreateLogList();

            var allEntriesPaged = _logService.GetAllEntriesPaged(1);

            allEntriesPaged.Should().HaveCount(10);
        }

        [Fact]
        public void LogService_GetAllEntriesPaged_ShouldReturnTheFirst10Items()
        {
            var list = CreateLogList();

            var allEntriesPaged = _logService.GetAllEntriesPaged(1);

            allEntriesPaged.Should().BeEquivalentTo(list.Take(10));
        }

        [Fact]
        public void LogService_GetAllEntriesPaged_PageSelectedShouldPageTheResults()
        {
            var list = CreateLogList();

            var allEntriesPaged = _logService.GetAllEntriesPaged(2);

            allEntriesPaged.Should().BeEquivalentTo(list.Skip(10).Take(10));
        }

        [Fact]
        public void LogService_GetAllErrors_ShouldOnlyReturnErrors()
        {
            var list =
                Enumerable.Range(1, 20)
                          .Select(
                              i =>
                              new Log
                                  {
                                      Message = i.ToString(),
                                      Error = new Error(),
                                      Type = i <= 5 ? LogEntryType.Error : LogEntryType.Audit
                                  })
                          .ToList();
            Session.Transact(session => list.ForEach(log => session.Save(log)));

            var allEntriesPaged = _logService.GetAllErrors(1);

            allEntriesPaged.Should().BeEquivalentTo(list.Take(5));
        }

        [Fact]
        public void LogService_GetAllAudits_ShouldOnlyReturnAudits()
        {
            var list =
                Enumerable.Range(1, 20)
                          .Select(
                              i =>
                              new Log
                                  {
                                      Message = i.ToString(),
                                      Error = new Error(),
                                      Type = i <= 5 ? LogEntryType.Audit : LogEntryType.Error
                                  })
                          .ToList();
            Session.Transact(session => list.ForEach(log => session.Save(log)));

            var allEntriesPaged = _logService.GetAllAudits(1);

            allEntriesPaged.Should().BeEquivalentTo(list.Take(5));
        }

        private static List<Log> CreateLogList()
        {
            var logList = Enumerable.Range(1, 20).Select(i => new Log {Message = i.ToString(), Error = new Error()}).ToList();
            Session.Transact(session => logList.ForEach(log => session.Save(log)));
            return logList;
        }

        [Fact]
        public void FactMethodName()
        {
            
        }
    }
}