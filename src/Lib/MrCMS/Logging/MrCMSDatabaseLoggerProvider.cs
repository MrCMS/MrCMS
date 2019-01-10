using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public MrCMSDatabaseLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            var httpContext = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (httpContext == null)
                return NullLogger.Instance;
            return httpContext.RequestServices.GetRequiredService<MrCMSDatabaseLogger>();
        }
    }
}