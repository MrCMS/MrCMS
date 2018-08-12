using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using ISession = NHibernate.ISession;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLogger : ILogger
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICurrentSiteLocator _currentSiteLocator;
        private readonly ISession _session;

        public MrCMSDatabaseLogger(IHttpContextAccessor contextAccessor, ICurrentSiteLocator currentSiteLocator, ISession session)
        {
            _contextAccessor = contextAccessor;
            _currentSiteLocator = currentSiteLocator;
            _session = session;
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;
            var log = new Log
            {
                ExceptionData = GetExceptionData(exception),
                RequestData = GetRequestData(_contextAccessor.HttpContext),
                Message = exception.Message,
                Detail = exception.StackTrace,
                Site = _currentSiteLocator.GetCurrentSite()
            };
            _session.Transact(session => session.Save(log));
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
            return logLevel == LogLevel.Critical || logLevel == LogLevel.Error || logLevel == LogLevel.Warning;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}