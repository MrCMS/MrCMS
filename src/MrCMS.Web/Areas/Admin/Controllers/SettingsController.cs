using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SettingsController : MrCMSAdminController
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IServiceProvider _serviceProvider;

        public SettingsController(IConfigurationProvider configurationProvider, IServiceProvider serviceProvider)
        {
            _configurationProvider = configurationProvider;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        [Acl(typeof(SiteSettingsACL), SiteSettingsACL.View)]
        public async Task<ViewResult> Index()
        {
            var allSiteSettings = await _configurationProvider.GetAllSiteSettings();
            var settings = allSiteSettings.FindAll(arg => arg.RenderInSettings);
            settings.ForEach(@base => @base.SetViewData(_serviceProvider, ViewData));
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [Acl(typeof(SiteSettingsACL), SiteSettingsACL.Save)]
        public RedirectToActionResult Index_Post(
            [ModelBinder(typeof(SiteSettingsModelBinder))]
            List<SiteSettingsBase> settings) 
        {
            settings.ForEach(s => _configurationProvider.SaveSettings(s));
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ViewResult> FileSystem()
        {
            return View(await _configurationProvider.GetSiteSettings<FileSystemSettings>());
        }

        [HttpPost]
        public async Task<RedirectToActionResult> FileSystem(FileSystemSettings settings) 
        {
            await _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Settings saved.".AsResource(HttpContext));
            return RedirectToAction("FileSystem");
        }

     
    }
}