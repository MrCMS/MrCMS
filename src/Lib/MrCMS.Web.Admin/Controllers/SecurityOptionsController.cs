using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class SecurityOptionsController : MrCMSAdminController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IRoleAdminService _roleAdminService;

        public SecurityOptionsController(ISystemConfigurationProvider configurationProvider,
            IRoleAdminService roleAdminService)
        {
            _configurationProvider = configurationProvider;
            _roleAdminService = roleAdminService;
        }

        public async Task<ViewResult> Index()
        {
            ViewData["roles"] = await _roleAdminService.GetAllRoles();
            ViewData["auth-role-settings"] = _configurationProvider.GetSystemSettings<AuthRoleSettings>();
            ViewData["security-settings"] = _configurationProvider.GetSystemSettings<SecuritySettings>();

            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Index(AuthRoleSettings settings)
        {
            await _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Settings(SecuritySettings settings)
        {
            await _configurationProvider.SaveSettings(settings);
            return RedirectToAction("Index");
        }
    }
}