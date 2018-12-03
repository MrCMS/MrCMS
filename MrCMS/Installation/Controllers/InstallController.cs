using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Installation.Models;
using MrCMS.Installation.Services;

namespace MrCMS.Installation.Controllers
{
    public class InstallController : Controller
    {
        private readonly IInstallationService _installationService;
        private readonly IApplicationLifetime _applicationLifetime;

        public InstallController(IInstallationService installationService, IApplicationLifetime applicationLifetime)
        {
            _installationService = installationService;
            _applicationLifetime = applicationLifetime;
        }

        public IActionResult Setup()
        {
            var installModel = TempData.Get<InstallModel>();
            // if it is a new setup
            if (installModel == null)
            {
                ModelState.Clear();
                installModel = new InstallModel
                {
                    SiteUrl = Request.Host.ToString(),
                    AdminEmail = "admin@yoursite.com",
                    DatabaseConnectionString = "",
                    DatabaseProvider = typeof(SqlServer2012Provider).FullName,
                    SqlAuthenticationType = SqlAuthenticationType.SQL,
                    SqlConnectionInfo = SqlConnectionInfo.Values,
                    SqlServerCreateDatabase = false,
                };
            }

            ViewData["provider-types"] = _installationService.GetProviderTypes();
            return View(installModel);
        }

        [HttpPost]
        public IActionResult Setup(InstallModel installModel)
        {
            var installationResult = _installationService.Install(installModel);

            if (!installationResult.Success)
            {
                TempData.Set(installationResult);
                TempData.Set(installModel);

                return RedirectToAction("Setup");
            }
            _applicationLifetime.StopApplication();
            return Redirect("~");
        }
    }
}