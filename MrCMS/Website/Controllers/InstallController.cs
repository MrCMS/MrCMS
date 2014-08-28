using System.Web.Mvc;
using MrCMS.DbConfiguration;
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


            // if it is a new setup
            if (string.IsNullOrWhiteSpace(installModel.DatabaseProvider))
            {
                ModelState.Clear();
                installModel = new InstallModel
                            {
                                SiteUrl = Request.Url.Authority,
                                AdminEmail = "admin@yoursite.com",
                                DatabaseConnectionString = "",
                                DatabaseProvider = typeof(SqlServer2012Provider).FullName,
                                SqlAuthenticationType = SqlAuthenticationType.SQL,
                                SqlConnectionInfo = SqlConnectionInfo.Values,
                                SqlServerCreateDatabase = false,
                            };
            }
            return ShowPage(installModel);
        }

        private ActionResult ShowPage(InstallModel installModel)
        {
            ViewData["provider-types"] = _installationService.GetProviderTypes();
            return View(installModel);
        }

        [HttpPost]
        public ActionResult Setup(InstallModel installModel)
        {
            if (CurrentRequestData.DatabaseIsInstalled)
                return Redirect("~");

            //set page timeout to 5 minutes
            Server.ScriptTimeout = 300;

            var installationResult = _installationService.Install(installModel);

            if (!installationResult.Success)
            {
                ViewData["installationResult"] = installationResult;

                return ShowPage(installModel);
            }
            return Redirect("~");
        }
    }
}