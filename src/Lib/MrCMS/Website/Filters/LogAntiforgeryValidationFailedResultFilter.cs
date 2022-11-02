using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MrCMS.Website.Filters;

public class LogAntiforgeryValidationFailedResultFilter : IAlwaysRunResultFilter
{
    private readonly ILogger<LogAntiforgeryValidationFailedResultFilter> _logger;

    public LogAntiforgeryValidationFailedResultFilter(ILogger<LogAntiforgeryValidationFailedResultFilter> logger)
    {
        _logger = logger;
    }
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is IAntiforgeryValidationFailedResult result)
        {
            _logger.LogError("Antiforgery token error");
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    { }
}