using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;

namespace MrCMS.Website.Filters;

public class MrCMSAutoValidateAntiforgeryTokenAuthorizationFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
{
    private readonly IAntiforgery _antiforgery;
    private readonly ILogger _logger;

    public MrCMSAutoValidateAntiforgeryTokenAuthorizationFilter(IAntiforgery antiforgery,
        ILoggerFactory loggerFactory)
    {
        _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
        {
            // _logger.NotMostEffectiveFilter(typeof(IAntiforgeryPolicy));
            return;
        }

        if (ShouldValidate(context))
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(context.HttpContext);
            }
            catch (AntiforgeryValidationException exception)
            {
                _logger.LogError(exception, "Antiforgery token validation failed.");
                var referrer = context.HttpContext.Request.RefererLocal();
                if (!string.IsNullOrEmpty(referrer))
                {
                    var query = context.HttpContext.Request.QueryString.Add("requestError", "1");
                    var uri = new UriBuilder
                    {
                        Path = referrer,
                        Query = query.ToString() ?? string.Empty
                    };
                    context.Result = new LocalRedirectResult(uri.Uri.PathAndQuery);
                }
                else
                {
                    context.Result = new BadRequestObjectResult(exception.Message);
                }
            }
        }
    }

    private bool ShouldValidate(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var method = context.HttpContext.Request.Method;
        if (HttpMethods.IsGet(method) ||
            HttpMethods.IsHead(method) ||
            HttpMethods.IsTrace(method) ||
            HttpMethods.IsOptions(method))
        {
            return false;
        }

        // Anything else requires a token.
        return true;
    }
}