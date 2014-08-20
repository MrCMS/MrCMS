using System.Web.Mvc;
using MrCMS.DbConfiguration.Configuration;
using MrCMS.Installation;

namespace MrCMS.Website.Controllers
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

            var installationResult = _installationService.Install(model);

            if (!installationResult.Success)
            {
                ViewData["installationResult"] = installationResult;
                return View(model);
            }
            return Redirect("~");
        }
    }
}