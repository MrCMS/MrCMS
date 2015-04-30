using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SystemSettingsController : MrCMSAdminController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public SystemSettingsController(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(SystemSettingsACL), SystemSettingsACL.View)]
        public ViewResult Index()
        {
            var settings = _configurationProvider.GetAllSystemSettings().FindAll(arg => arg.RenderInSettings);
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [MrCMSACLRule(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public RedirectToRouteResult Index_Post([IoCModelBinder(typeof(SystemSettingsModelBinder))]List<SystemSettingsBase> settings)
        {
            settings.ForEach(s => _configurationProvider.SaveSettings(s));
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }
        
    }
}