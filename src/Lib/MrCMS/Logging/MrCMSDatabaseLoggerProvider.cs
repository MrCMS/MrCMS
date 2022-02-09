using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLoggerProvider : ILoggerProvider
    {
        private readonly ISessionFactory _session;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ConcurrentDictionary<string, MrCMSDatabaseLogger> _loggers =
            new ConcurrentDictionary<string, MrCMSDatabaseLogger>();

        public MrCMSDatabaseLoggerProvider(ISessionFactory session, IHttpContextAccessor httpContextAccessor)
        {
            _session = session;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName,
                s => new MrCMSDatabaseLogger(_session, _httpContextAccessor));
        }
    }
}