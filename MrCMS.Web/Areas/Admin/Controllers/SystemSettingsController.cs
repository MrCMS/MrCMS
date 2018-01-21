using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SystemSettingsController : MrCMSAdminController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly ITestSmtpSettings _testSmtpSettings;

        public SystemSettingsController(ISystemConfigurationProvider configurationProvider, ITestSmtpSettings testSmtpSettings)
        {
            _configurationProvider = configurationProvider;
            _testSmtpSettings = testSmtpSettings;
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

        [HttpGet]
        [MrCMSACLRule(typeof(SystemSettingsACL), SystemSettingsACL.View)]
        public ViewResult Mail()
        {
            var mailSettings = _configurationProvider.GetSystemSettings<MailSettings>();
            return View(mailSettings);
        }

        [HttpPost]
        [MrCMSACLRule(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public RedirectToRouteResult Mail(MailSettings settings)
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Mail settings saved.");
            return RedirectToAction("Mail");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public RedirectToRouteResult TestMailSettings(TestEmailInfo info)
        {
            var result = _testSmtpSettings.TestSettings(_configurationProvider.GetSystemSettings<MailSettings>(), info);
            if (result)
                TempData.SuccessMessages().Add("Mail settings saved and email sent.");
            else
                TempData.ErrorMessages().Add("An error occurred, check the log for details.");
            return RedirectToAction("Mail");
        }
    }

}