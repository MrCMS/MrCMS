using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Logging;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Controllers
{
    //[TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    public class IS4AdminHomeController : MrCMSAdminController
    {
        private readonly ILogger<IS4AdminHomeController> _logger;
       // private readonly IApiDescriptionGroupCollectionProvider _apiExplorer;
       // private readonly ITestRepository _testService;
        public IS4AdminHomeController(ILogger<IS4AdminHomeController> logger)
        {
            //  _apiExplorer = apiExplorer;
            // _testService = testService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //  return View(_apiExplorer);
            /// return Content(_testService.GetTestValue());
              return View();
        }


        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
            return LocalRedirect(returnUrl);
        }

        public IActionResult Error()
        {
            // Get the details of the exception that occurred
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if (exceptionFeature == null) return View();

            // Get which route the exception occurred at
            string routeWhereExceptionOccurred = exceptionFeature.Path;

            // Get the exception that occurred
            Exception exceptionThatOccurred = exceptionFeature.Error;

            // Log the exception
            _logger.LogError(exceptionThatOccurred, $"Exception at route {routeWhereExceptionOccurred}");

            return View();
        }
    }
}
