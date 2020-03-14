using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Services.Resources;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class SystemSettingsController : MrCMSAdminController
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly ITestSmtpSettings _testSmtpSettings;
        private readonly IStringResourceProvider _resourceProvider;

        public SystemSettingsController(ISystemConfigurationProvider configurationProvider, ITestSmtpSettings testSmtpSettings, IStringResourceProvider resourceProvider)
        {
            _configurationProvider = configurationProvider;
            _testSmtpSettings = testSmtpSettings;
            _resourceProvider = resourceProvider;
        }

        [HttpGet]
        [Acl(typeof(SystemSettingsACL), SystemSettingsACL.View)]
        public ViewResult Index()
        {
            var settings = _configurationProvider.GetAllSystemSettings().FindAll(arg => arg.RenderInSettings);
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [Acl(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public RedirectToActionResult Index_Post(
            [ModelBinder(typeof(SystemSettingsModelBinder))]
            List<SystemSettingsBase> settings)
        {
            settings.ForEach(s => _configurationProvider.SaveSettings(s));
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(SystemSettingsACL), SystemSettingsACL.View)]
        public ViewResult Mail()
        {
            var mailSettings = _configurationProvider.GetSystemSettings<MailSettings>();
            return View(mailSettings);
        }

        [HttpPost]
        [Acl(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public async Task<RedirectToActionResult> Mail(MailSettings settings)
        {
            await _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Mail settings saved.");
            return RedirectToAction("Mail");
        }

        [HttpPost]
        [Acl(typeof(SystemSettingsACL), SystemSettingsACL.Save)]
        public async Task<RedirectToActionResult> TestMailSettings(TestEmailInfo info)
        {
            var result = _testSmtpSettings.TestSettings(await _configurationProvider.GetSystemSettings<MailSettings>(), info);
            if (result)
                TempData.SuccessMessages().Add(_resourceProvider.GetValue("Admin - Test email - Success", "Email sent."));
            else
                TempData.ErrorMessages().Add(_resourceProvider.GetValue("Admin - Test email - Failure", "An error occurred, check the log for details."));
            return RedirectToAction("Mail");
        }
    }
}