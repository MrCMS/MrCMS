// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using MrCMS.Entities.People;
using MrCMS.Services.Resources;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Services;
using NHibernate;

namespace MrCMS.Web.IdentityServer
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _logger;
        private readonly ISession _session;
        private readonly IDeveloperIS4Service _service;
        private readonly IStringResourceProvider _stringLocalizer;
        private readonly IStringLocalizer<HomeController> _localizer;

        public HomeController(IIdentityServerInteractionService interaction, IHostingEnvironment environment, ILogger<HomeController> logger, ISession session, 
            IDeveloperIS4Service service, IStringResourceProvider stringLocalizer, IStringLocalizer<HomeController> localizer)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
            _session = session;
            _service = service;
            _stringLocalizer = stringLocalizer;
            _localizer = localizer;

        }

        public IActionResult Index()
        {
            var session = HttpContext.RequestServices.GetRequiredService<ISession>();
            var result = session.QueryOver<Client>().Where(i => i.Id == 1).List();
            var naming = result.Any() ? result.FirstOrDefault()?.ClientName : "Faithiecute";
            _logger.LogInformation(naming);
            if (_environment.IsDevelopment())
            {
                //_service.InitializeDatabaseData();
                // only show in development
                return View();

            }

            //  var grantType = _session.QueryOver<ClientGrantType>().Where(x => x.).List();

            //  var session = HttpContext.RequestServices.GetRequiredService<IStatelessSession>();

            _logger.LogInformation("Homepage is disabled in production. Returning 404.");
            return NotFound();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }
}