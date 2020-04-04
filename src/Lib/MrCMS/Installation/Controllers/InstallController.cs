using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MrCMS.DbConfiguration;
using MrCMS.Helpers;
using MrCMS.Installation.Models;
using MrCMS.Installation.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;

// ReSharper disable Mvc.ViewNotResolved - Views compiled in MrCMS.DLL

namespace MrCMS.Installation.Controllers
{
    public class InstallController : Controller
    {
        private readonly IInstallationService _installationService;
        private readonly IGetSystemCultureInfo _getSystemCultureInfo;

        public InstallController(IInstallationService installationService, IGetSystemCultureInfo getSystemCultureInfo)
        {
            _installationService = installationService;
            _getSystemCultureInfo = getSystemCultureInfo;
        }

        public ViewResult RequiresSettings()
        {
            return View("Installation/Views/Install/RequiresSettings.cshtml");
        }

        public ViewResult RequiresMigrations()
        {
            return View("Installation/Views/Install/RequiresMigrations.cshtml");
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
                    CultureOptions = _getSystemCultureInfo.GetCultureOptions()
                };
            }
            
            ViewData["provider-types"] = _installationService.GetProviderTypes();
            return View("Installation/Views/Install/Setup.cshtml", installModel);
        }

        [HttpPost]
        public async Task<IActionResult> Setup(InstallModel installModel)
        {
            var installationResult = await _installationService.Install(installModel);

            if (!installationResult.Success)
            {
                TempData.Set(installationResult);
                TempData.Set(installModel);
                return RedirectToAction("Setup");
            }

            //_applicationLifetime.StopApplication();
            return View("Installation/Views/Install/Success.cshtml");
        }

        public IActionResult Redirect()
        {
            return Redirect("~/");
        }
    }
}