using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Logging
{
    public class MrCMSDatabaseLogger : ILogger
    {
        private readonly ISessionFactory _factory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MrCMSDatabaseLogger(ISessionFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _factory = factory;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var log = new Log
            {
                LogLevel = logLevel,
                ExceptionData = GetExceptionData(exception),
                Message = exception != null ? exception.Message : formatter(state, exception),
                Detail = exception?.StackTrace
            };

            using var statelessSession = _factory.OpenStatelessSession();

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var site = httpContext.RequestServices.GetService<ICurrentSiteLocator>()?.GetCurrentSite();
                log.RequestData = GetRequestData(httpContext);
                log.Site = site;
            }

            log.CreatedOn = DateTime.UtcNow;
            log.UpdatedOn = DateTime.UtcNow;
            log.IsDeleted = false;

            try
            {
                statelessSession.Insert(log);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            return logLevel is LogLevel.Critical or LogLevel.Error;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}