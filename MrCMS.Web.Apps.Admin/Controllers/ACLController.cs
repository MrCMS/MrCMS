using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL;
using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        [Acl(typeof(AclAdminACL), AclAdminACL.View)]
        public ViewResult Index()
        {
            ViewData["settings"] = _aclSettings;
            return View(_aclService.GetACLModel());
        }

        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public RedirectToActionResult Index(
            [ModelBinder(typeof(ACLUpdateModelBinder))]
            List<ACLUpdateRecord> model)
        {
            _aclService.UpdateACL(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public ActionResult Disable()
        {
            _aclSettings.ACLEnabled = false;
            _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public ActionResult Enable()
        {
            _aclSettings.ACLEnabled = true;
            _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }
    }
}