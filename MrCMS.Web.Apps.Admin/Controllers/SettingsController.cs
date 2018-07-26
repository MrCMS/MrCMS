using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL.Rules;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class SettingsController : MrCMSAdminController
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly ISession _session;

        public SettingsController(IConfigurationProvider configurationProvider, ISession session)
        {
            _configurationProvider = configurationProvider;
            _session = session;
        }

        [HttpGet]
        [Acl(typeof(SiteSettingsACL), SiteSettingsACL.View)]
        public ViewResult Index()
        {
            var settings = _configurationProvider.GetAllSiteSettings().FindAll(arg => arg.RenderInSettings);
            settings.ForEach(@base => @base.SetViewData(_session, ViewData));
            return View(settings);
        }

        [HttpPost]
        [ActionName("Index")]
        [Acl(typeof(SiteSettingsACL), SiteSettingsACL.Save)]
        public RedirectToActionResult Index_Post(
            //[IoCModelBinder(typeof(SiteSettingsModelBinder))]
            List<SiteSettingsBase> settings) // TODO: model-binding
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
        public RedirectToActionResult FileSystem(
            //[IoCModelBinder(typeof(FileSystemSettingsModelBinder))]
            FileSystemSettings settings) // TODO: model-binding
        {
            _configurationProvider.SaveSettings(settings);
            TempData.SuccessMessages().Add("Settings saved.".AsResource(HttpContext));
            return RedirectToAction("FileSystem");
        }

     
    }
}