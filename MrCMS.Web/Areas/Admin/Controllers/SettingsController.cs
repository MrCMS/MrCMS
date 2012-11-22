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
        private readonly List<Type> _settingTypes;

        public SettingsController(ISession session)
        {
            _session = session;
            _settingTypes = TypeHelper.GetAllConcreteTypesAssignableFrom<ISettings>();
        }

        [HttpGet]
        public ViewResultBase Index()
        {
            var settingses = _settingTypes.Select(settingType =>
                                                     {
                                                         var settings = MrCMSApplication.Get(settingType) as ISettings;
                                                         if (settings != null) settings.SetViewData(_session, ViewData);
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