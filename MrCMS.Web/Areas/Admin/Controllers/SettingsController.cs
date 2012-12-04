using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.Mvc;
using MrCMS.Helpers;
using System.Linq;
using MrCMS.Settings;
using MrCMS.Website;
using MrCMS.Website.Binders;
using NHibernate;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SettingsController : AdminController
    {
        private readonly ISession _session;
        private readonly ISettingService _settingService;

        public SettingsController(ISession session, ISettingService settingService)
        {
            _session = session;
            _settingService = settingService;
        }

        [HttpGet]
        public ViewResultBase Index()
        {
            var settingses = _settingService
                .GetAllISettings()
                .Select(settings =>
                            {
                                if (settings != null)
                                    settings.SetViewData(_session, ViewData);
                                return settings;
                            }).ToList();

            return View(settingses);
        }

        [HttpPost]
        public ActionResult Index([ModelBinder(typeof(SettingModelBinder))]List<ISettings> settings)
        {
            settings.ForEach(s => s.Save());

            return RedirectToAction("Index", "Settings");
        }
    }
}