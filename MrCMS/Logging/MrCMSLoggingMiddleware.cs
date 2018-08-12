using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MrCMS.Logging
{
    public class MrCMSLoggingMiddleware : IMiddleware
    {
        private readonly ILoggerFactory _loggerFactory;

        public MrCMSLoggingMiddleware(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                _loggerFactory.CreateLogger("pipeline").Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }
    }
}