using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Installation;
using MrCMS.Services;
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
            if (CurrentRequestData.DatabaseIsInstalled)
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
                    BodyContent = "Welcome to Mr CMS",
                    RevealInNavigation = true,
                    PublishOn = DateTime.Now,
                    Layout = model.BaseLayout
                };
            model.Error404 = new TextPage
                {
                    Name = "404",
                    UrlSegment = "404",
                    BodyContent = "Sorry, this page cannot be found.",
                    RevealInNavigation = false,
                    PublishOn = DateTime.UtcNow,
                    Layout = model.BaseLayout
                };
            model.Error403 = new TextPage
                {
                    Name = "403",
                    UrlSegment = "403",
                    BodyContent = "Sorry, you are not authorized to view this page.",
                    RevealInNavigation = false,
                    PublishOn = DateTime.UtcNow,
                    Layout = model.BaseLayout
                };
            model.Error500 = new TextPage
                {
                    Name = "500",
                    UrlSegment = "500",
                    BodyContent = "Sorry, there has been an error.",
                    RevealInNavigation = false,
                    PublishOn = DateTime.Now,
                    Layout = model.BaseLayout
                };

            model.Page2 = new TextPage
            {
                Name = "Page 2",
                UrlSegment = "page-2",
                BodyContent = "Just another page!",
                RevealInNavigation = true,
                PublishOn = DateTime.Now,
                Layout = model.BaseLayout
            };

            model.Page3 = new TextPage
            {
                Name = "Contact us",
                UrlSegment = "contact-us",
                BodyContent = "Contact us at www.mrcms.co.uk.",
                RevealInNavigation = true,
                PublishOn = DateTime.Now,
                Layout = model.BaseLayout
            };

            
        }
    }
}