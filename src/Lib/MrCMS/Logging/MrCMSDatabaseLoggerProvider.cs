using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLoggerProvider : ILoggerProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly ConcurrentDictionary<string, MrCMSDatabaseLogger> _loggers =
            new ConcurrentDictionary<string, MrCMSDatabaseLogger>();

        public MrCMSDatabaseLoggerProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName,
                s => new MrCMSDatabaseLogger(_contextAccessor));
        }
    }
}