using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Data;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLogger : ILogger
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public MrCMSDatabaseLogger(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            var context = _contextAccessor.HttpContext;
            var currentSiteLocator = context.RequestServices.GetRequiredService<ICurrentSiteLocator>();
            var repository = context.RequestServices.GetRequiredService<IRepository<Log>>();
            var log = new Log
            {
                LogLevel = logLevel,
                ExceptionData = GetExceptionData(exception),
                RequestData = GetRequestData(context),
                Message = formatter(state, exception),
                Detail = exception?.StackTrace,
                Site = currentSiteLocator.GetCurrentSite().GetAwaiter().GetResult()
            };
            repository.Add(log).GetAwaiter().GetResult();
        }

        private string GetRequestData(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;

            var requestData = new ErrorContextData
            {
                Uri = httpContext.Request.GetDisplayUrl(),
                User = httpContext.User.Identity.Name,
                UserAgent = httpContext.Request.UserAgent(),
                IPAddress = httpContext.Request.GetCurrentIP(),
                Headers = httpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                Method = httpContext.Request.Method
            };

            return JsonConvert.SerializeObject(requestData);
        }

        private static string GetExceptionData(Exception exception)
        {
            if (exception == null)
                return null;
            try
            {
                return JsonConvert.SerializeObject(exception);
            }
            catch
            {
                return JsonConvert.SerializeObject(new
                {
                    exception.Message,
                    exception.StackTrace,
                    exception.Source
                });
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _contextAccessor.HttpContext != null &&
                   (logLevel == LogLevel.Critical || logLevel == LogLevel.Error || logLevel == LogLevel.Warning);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}