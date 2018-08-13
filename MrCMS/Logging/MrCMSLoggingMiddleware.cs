using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MrCMS.Logging
{
    public class MrCMSLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<MrCMSLoggingMiddleware> _logger;

        public MrCMSLoggingMiddleware(ILogger<MrCMSLoggingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, exception.Message);
                throw;
            }
        }
    }
}