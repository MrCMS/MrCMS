using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using MrCMS.ACL;
using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ACLController : MrCMSAdminController
    {
        private readonly IACLService _aclService;
        private readonly ACLSettings _aclSettings;
        private readonly IConfigurationProvider _configurationProvider;

        public ACLController(IACLService aclService, ACLSettings aclSettings, IConfigurationProvider configurationProvider)
        {
            _aclService = aclService;
            _aclSettings = aclSettings;
            _configurationProvider = configurationProvider;
        }

        [HttpGet]
        [MrCMSACLRule(typeof(AclAdminACL), AclAdminACL.View)]
        public ViewResult Index()
        {
            ViewData["settings"] = _aclSettings;
            return View(_aclService.GetACLModel());
        }

        [HttpPost]
        [MrCMSACLRule(typeof(AclAdminACL), AclAdminACL.Edit)]
        public RedirectToRouteResult Index([IoCModelBinder(typeof(ACLUpdateModelBinder))] List<ACLUpdateRecord> model)
        {
            _aclService.UpdateACL(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(AclAdminACL), AclAdminACL.Edit)]
        public ActionResult Disable()
        {
            _aclSettings.ACLEnabled = false;
            _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [MrCMSACLRule(typeof(AclAdminACL), AclAdminACL.Edit)]
        public ActionResult Enable()
        {
            _aclSettings.ACLEnabled = true;
            _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }
    }
}