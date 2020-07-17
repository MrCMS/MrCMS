using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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
            ViewData["auth-role-settings"] = _configurationProvider.GetSystemSettings<AuthRoleSettings>();
            ViewData["security-settings"] = _configurationProvider.GetSystemSettings<SecuritySettings>();

            return View();
        }

        [HttpPost]
        public RedirectToActionResult Index(AuthRoleSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Settings(SecuritySettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}