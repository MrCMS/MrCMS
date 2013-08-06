using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Installation;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class InstallController : MrCMSUIController
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
            if (CurrentRequestData.DatabaseIsInstalled)
                return Redirect("~");

            //set page timeout to 5 minutes
            Server.ScriptTimeout = 300;

            // if it is a new setup
            if (installModel.DatabaseType == DatabaseType.Auto)
            {
                ModelState.Clear();
                installModel = new InstallModel
                            {
                                SiteUrl = Request.Url.Authority,
                                AdminEmail = "admin@yoursite.com",
                                DatabaseConnectionString = "",
                                DatabaseType = DatabaseType.MsSql,
                                SqlAuthenticationType = "sqlauthentication",
                                SqlConnectionInfo = "sqlconnectioninfo_values",
                                SqlServerCreateDatabase = false,
                            };
            }
            return View(installModel);
        }

        [HttpPost]
        public ActionResult Setup(InstallModel model)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
                return Redirect("~");

            //set page timeout to 5 minutes
            Server.ScriptTimeout = 300;
            SetInitialWebpages(model);

            var installationResult = _installationService.Install(model);

            if (!installationResult.Success)
            {
                ViewData["installationResult"] = installationResult;
                return View(model);
            }
            return Redirect("~");
        }

        private void SetInitialWebpages(InstallModel model)
        {
            model.BaseLayout = new Layout
                                   {
                                       Name = "Base Layout",
                                       UrlSegment = "~/Apps/Core/Views/Shared/_BaseLayout.cshtml",
                                       LayoutAreas = new List<LayoutArea>()
                                   };
            model.HomePage = new TextPage
                {
                    Name = "Home",
                    UrlSegment = "home",
                    BodyContent = "<h1>Mr CMS</h1> <p>Welcome to Mr CMS, the only CMS you will need.</p><p> Turn on inline editing above, then click here. Pretty cool huh? </p>",
                    RevealInNavigation = true,
                };
            model.Error404 = new TextPage
                {
                    Name = "404",
                    UrlSegment = "404",
                    BodyContent = "<h1>404</h1><p>Sorry, this page cannot be found.</p>",
                    RevealInNavigation = false,
                };
            model.Error403 = new TextPage
                {
                    Name = "403",
                    UrlSegment = "403",
                    BodyContent = "<h1>403</h1><p>Sorry, you are not authorized to view this page.</p>",
                    RevealInNavigation = false,
                };
            model.Error500 = new TextPage
                {
                    Name = "500",
                    UrlSegment = "500",
                    BodyContent = "<h1>500</h1><p>Sorry, there has been an error.</p>",
                    RevealInNavigation = false,
                };

            model.Page2 = new TextPage
            {
                Name = "Page 2",
                UrlSegment = "page-2",
                BodyContent = "<h1>Another page</h1><p>Just another page!</p>",
                RevealInNavigation = true,
            };

            model.Page3 = new TextPage
            {
                Name = "Contact us",
                UrlSegment = "contact-us",
                BodyContent = "<h1>Contact</h1>Contact us at www.mrcms.com (coming soon).",
                RevealInNavigation = true,
            };
        }
    }
}