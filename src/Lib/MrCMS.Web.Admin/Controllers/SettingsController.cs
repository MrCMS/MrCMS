using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Helpers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;

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
        public RedirectToActionResult Index_Post(
            [ModelBinder(typeof(SiteSettingsModelBinder))]
            List<SiteSettingsBase> settings) 
        {
            settings.ForEach(s => _configurationProvider.SaveSettings(s));
            TempData["settings-saved"] = true;
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult FileSystem()
        {
            return View(_configurationProvider.GetSiteSettings<FileSystemSettings>());
        }

        [HttpPost]
        public RedirectToActionResult FileSystem(FileSystemSettings settings) 
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Settings saved.".AsResource(HttpContext));
            return RedirectToAction("FileSystem");
        }

     
    }
}