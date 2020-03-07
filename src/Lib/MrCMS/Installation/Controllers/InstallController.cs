using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Installation.Models;
using MrCMS.Installation.Services;

namespace MrCMS.Installation.Controllers
{
    public class InstallController : Controller
    {
        private readonly IInstallationService _installationService;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public InstallController(IInstallationService installationService, IHostApplicationLifetime applicationLifetime)
        {
            _installationService = installationService;
            _applicationLifetime = applicationLifetime;
        }

        public IActionResult Setup()
        {
            if (_installationService.DatabaseIsInstalled())
            {
                return View("Success");
            }
            var installModel = TempData.Get<InstallModel>();
            // if it is a new setup
            if (installModel == null)
            {
                ModelState.Clear();
                installModel = new InstallModel
                {
                    SiteUrl = Request.Host.ToString(),
                    AdminEmail = "admin@yoursite.com",
                    //DatabaseConnectionString = "",
                    ////DatabaseProvider = typeof(SqlServer2012Provider).FullName,
                    //SqlAuthenticationType = SqlAuthenticationType.SQL,
                    //SqlConnectionInfo = SqlConnectionInfo.Values,
                    //SqlServerCreateDatabase = false,
                };
            }

            ViewData["provider-types"] = _installationService.GetProviderTypes();
            return View("Installation/Views/Install/Setup.cshtml", installModel);
        }

        [HttpPost]
        public async Task<IActionResult> Setup(InstallModel installModel)
        {
            var installationResult =await _installationService.Install(installModel);

            if (!installationResult.Success)
            {
                TempData.Set(installationResult);
                TempData.Set(installModel);
                return RedirectToAction("Setup");
            }

            _applicationLifetime.StopApplication();
            return View("Installation/Views/Install/Success.cshtml");
        }

        public IActionResult Redirect()
        {
            return Redirect("~/");
        }
    }
}