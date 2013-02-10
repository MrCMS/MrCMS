using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Web;
using MrCMS.Installation;
using MrCMS.Web.Application.Pages;
using MrCMS.Website;

namespace MrCMS.Web.Controllers
{
    public class InstallController : Controller
    {
        private readonly IInstallationService _installationService;

        public InstallController(IInstallationService installationService)
        {
            _installationService = installationService;
        }

        [ActionName("Setup")]
        [HttpGet]
        public ActionResult Setup_Get(InstallModel installModel)
        {
            if (MrCMSApplication.DatabaseIsInstalled)
                return Redirect("~");

            //set page timeout to 5 minutes
            Server.ScriptTimeout = 300;

            InstallModel model = installModel.DatabaseType == DatabaseType.Auto
                                     ? new InstallModel
                                         {
                                             SiteUrl = Request.Url.Authority,
                                             AdminEmail = "admin@yoursite.com",
                                             DatabaseConnectionString = "",
                                             DatabaseType = DatabaseType.MsSql,
                                             SqlAuthenticationType = "sqlauthentication",
                                             SqlConnectionInfo = "sqlconnectioninfo_values",
                                             SqlServerCreateDatabase = false,
                                         }
                                     : installModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult Setup(InstallModel model)
        {
            if (MrCMSApplication.DatabaseIsInstalled)
                return Redirect("~");

            //set page timeout to 5 minutes
            Server.ScriptTimeout = 300;
            SetInitialWebpages(model);
            InstallationResult installationResult = _installationService.Install(model);
            if (!installationResult.Success)
                return View(model);
            else return Redirect("~");
        }

        private void SetInitialWebpages(InstallModel model)
        {
            model.HomePage = new TextPage
                {
                    Name = "Home",
                    UrlSegment = "home",
                    RevealInNavigation = true
                };
            model.Error404 = new TextPage
                {
                    Name = "404",
                    UrlSegment = "404",
                    BodyContent = "Sorry, this page cannot be found.",
                    RevealInNavigation = false,
                    PublishOn = DateTime.UtcNow,
                };
            model.Error500 = new TextPage
                {
                    Name = "500",
                    UrlSegment = "500",
                    BodyContent = "Sorry, there has been an error.",
                    RevealInNavigation = false,
                    PublishOn = DateTime.Now,
                };
        }
    }
}