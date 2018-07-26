using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class SecurityOptionsController : MrCMSAdminController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IRoleAdminService _roleAdminService;

        public SecurityOptionsController(ISystemConfigurationProvider configurationProvider, IRoleAdminService roleAdminService)
        {
            _configurationProvider = configurationProvider;
            _roleAdminService = roleAdminService;
        }

        public ViewResult Index()
        {
            ViewData["roles"] = _roleAdminService.GetAllRoles().ToList();
            ViewData["settings"] = _configurationProvider.GetSystemSettings<AuthRoleSettings>();

            return View();
        }

        [HttpPost]
        public RedirectToActionResult Index(AuthRoleSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }

        public ActionResult Settings()
        {
            return PartialView(_configurationProvider.GetSystemSettings<SecuritySettings>());
        }

        [HttpPost]
        public ActionResult Settings(SecuritySettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}