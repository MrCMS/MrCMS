using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
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
        public ViewResult Index()
        {
            var settings = _configurationProvider.GetAllSiteSettings().FindAll(arg => arg.RenderInSettings);
            settings.ForEach(@base => @base.SetViewData(_serviceProvider, ViewData));
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [Acl(typeof(SiteSettingsACL), SiteSettingsACL.Save)]
        public async Task<RedirectToActionResult> Index_Post(
            [ModelBinder(typeof(SiteSettingsModelBinder))]
            List<SiteSettingsBase> settings)
        {
            foreach (var setting in settings)
            {
                await _configurationProvider.SaveSettings(setting);
            }
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult FileSystem()
        {
            return View(_configurationProvider.GetSiteSettings<FileSystemSettings>());
        }

        [HttpPost]
        public async Task<RedirectToActionResult> FileSystem(FileSystemSettings settings)
        {
            await _configurationProvider.SaveSettings(settings);
            TempData.AddSuccessMessage(await "Settings saved.".AsResource(HttpContext));
            return RedirectToAction("FileSystem");
        }
    }
}