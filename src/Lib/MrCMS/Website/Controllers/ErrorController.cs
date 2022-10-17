using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MrCMS.Helpers;
using MrCMS.Installation.Services;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Website.NotFound;

namespace MrCMS.Website.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ErrorController : MrCMSController
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessWebpageViews _processWebpageViews;

        // private readonly IGetCurrentUser _getCurrentUser;
        // private readonly IStringResourceProvider _stringResourceProvider;
        // private readonly ILogger<ErrorController> _logger;
        // private readonly INotFoundHandler _notFoundHandler;
        // private readonly SiteSettings _siteSettings;
        //
        // public ErrorController(IGetCurrentUser getCurrentUser, IStringResourceProvider stringResourceProvider,
        //     ILogger<ErrorController> logger, INotFoundHandler notFoundHandler, SiteSettings siteSettings)
        // {
        //     _getCurrentUser = getCurrentUser;
        //     _stringResourceProvider = stringResourceProvider;
        //     _logger = logger;
        //     _notFoundHandler = notFoundHandler;
        //     _siteSettings = siteSettings;
        // }
        public ErrorController(IServiceProvider serviceProvider, IProcessWebpageViews processWebpageViews)
        {
            _serviceProvider = serviceProvider;
            _processWebpageViews = processWebpageViews;
        }

        [Route("HandleStatusCode/{code}")]
        public async Task<ActionResult> HandleStatusCode(int code)
        {
            if (!_serviceProvider.GetRequiredService<IDatabaseCreationService>().IsDatabaseInstalled())
                return StatusCode(code);

            await _processWebpageViews.ProcessForDefault(ViewData);
            var logger = _serviceProvider.GetRequiredService<ILogger<ErrorController>>();

            if (code == 404)
            {
                var feature = HttpContext
                    .Features
                    .Get<IStatusCodeReExecuteFeature>();
                var result = await TryHandle404(Request, feature, logger);
                if (result.IsGone)
                {
                    code = 410;
                }
                else if (result.RedirectResult != null)
                {
                    logger.LogInformation("Redirecting to {Url}", result.RedirectResult.Url);
                    return result.RedirectResult;
                }
            }

            var stringResourceProvider = _serviceProvider.GetRequiredService<IStringResourceProvider>();
            var model = new StatusCodeModel
            {
                Code = code,
                Message = code switch
                {
                    404 => await stringResourceProvider.GetValue($"ui-{code}-message",
                        "Oops! This resource cannot be found."),
                    403 => await stringResourceProvider.GetValue($"ui-{code}-message",
                        "Oops! You do not have access to this page."),
                    410 => await stringResourceProvider.GetValue($"ui-{code}-message",
                        "Sorry, this resource no longer exists."),
                    _ => await stringResourceProvider.GetValue($"ui-{code}-message", "Sorry, an error has occurred.")
                }
            };
            Response.StatusCode = code;
            return View("HandleStatusCode", model);
        }

        private async Task<(bool IsGone, RedirectResult RedirectResult)> TryHandle404(HttpRequest request,
            IStatusCodeReExecuteFeature feature, ILogger<ErrorController> logger)
        {
            // we trim start to standardise with URL History
            string path = $"{feature.OriginalPathBase}{feature.OriginalPath}".TrimStart('/');
            var query = feature.OriginalQueryString;

            logger.LogDebug("404 Lookup - Path: {Path}, QueryString: {Query}", path, query);
            // try match exact - path + query
            var notFoundHandler = _serviceProvider.GetRequiredService<INotFoundHandler>();
            var (isGone, result, history) = await notFoundHandler.FindByPathAndQuery(path, query);

            // if we've handled it, either as gone, or as a redirect, return
            if (isGone)
                return (true, null);

            if (result != null)
                return (false, result);
            // if we've not handled it, but we know about it, increment if not ignored and return null
            if (history != null)
            {
                if (!history.IsIgnored)
                    await notFoundHandler.IncrementFailedLookupCount(history);
                return (false, null);
            }

            var siteSettings = _serviceProvider.GetRequiredService<SiteSettings>();
            var siteId = siteSettings.SiteId;

            // lookup to see if path and query match known routes
            result = await notFoundHandler.CheckKnownRoutes(path, query, siteId);
            if (result != null)
                return (false, result);

            // try match path and forward query string
            result = await notFoundHandler.FindByPathAndForwardQueryToPage(path, query);
            if (result != null)
                return (false, result);

            // add to known 404s
            if (siteSettings.Log404s)
                await notFoundHandler.AddHistoryRecord(path, query, request.Referer(), siteId);

            return (false, null);
        }

        [Route("error")]
        public async Task<ViewResult> Error()
        {
            await _processWebpageViews.ProcessForDefault(ViewData);

            var model = new ErrorModel();
            var currentUser = _serviceProvider.GetRequiredService<IGetCurrentUser>();
            var user = await currentUser.Get();

            var pathFeature = HttpContext
                .Features
                .Get<IExceptionHandlerPathFeature>();

            if (pathFeature == null)
            {
                model.Message = "An error occurred processing your request. Please try again.";
                return View(model);
            }

            var error = pathFeature.Error;
            if (user != null && user.IsAdmin && error != null)
            {
                model.Message =
                    $"An error occurred: {pathFeature.Error.Message}. The stack trace is: <br /> {error.StackTrace}";
                //todo look at logging the error here with further diagnostics e.g current user / path / variables about user
            }
            else
            {
                model.Message = "An error occurred processing your request. Please try again.";
            }

            return View(model);
        }

        public Task<ActionResult> Handle401()
        {
            return HandleStatusCode(401);
        }

        public Task<ActionResult> Handle403()
        {
            return HandleStatusCode(403);
        }

        public Task<ActionResult> Handle404()
        {
            return HandleStatusCode(404);
        }
    }
}