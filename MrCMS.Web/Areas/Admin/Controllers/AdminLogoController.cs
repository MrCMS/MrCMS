using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class AdminLogoController : MrCMSAdminController
    {

        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISession _session;

        public AdminLogoController(IConfigurationProvider configurationProvider, ISession session)
        {
            _configurationProvider = configurationProvider;
            _session = session;
        }
        [HttpGet]
        [MrCMSACLRule(typeof(AdminLogoSettingsACL), AdminLogoSettingsACL.View)]
        public ViewResult AdminLogoSettings()
        {
            var settings = _configurationProvider.GetSiteSettings<AdminLogoSettings>();
            return View(settings);
        }

        [HttpPost]
        [ActionName("AdminLogoSettings")]
        [MrCMSACLRule(typeof(AdminLogoSettingsACL), AdminLogoSettingsACL.Save)]
        public RedirectToRouteResult AdminLogoSettings_Post(AdminLogoSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            TempData["settings-saved"] = true;
            return RedirectToAction("AdminLogoSettings");
        }

        public ActionResult GetLogo()
        {
            var settings = _configurationProvider.GetSiteSettings<AdminLogoSettings>();
            return Content(string.IsNullOrEmpty(settings.MrCMSAdminLogo)?"/Areas/Admin/Content/Images/logo.png":settings.MrCMSAdminLogo);
        }
    }
}