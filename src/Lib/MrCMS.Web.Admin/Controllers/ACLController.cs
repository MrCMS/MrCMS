using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL;
using MrCMS.ACL.Rules;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class ACLController : MrCMSAdminController
    {
        private readonly IAclAdminService _aclService;
        private readonly ACLSettings _aclSettings;
        private readonly IConfigurationProvider _configurationProvider;

        public ACLController(IAclAdminService aclService, ACLSettings aclSettings, IConfigurationProvider configurationProvider)
        {
            _aclService = aclService;
            _aclSettings = aclSettings;
            _configurationProvider = configurationProvider;
        }

        public ViewResult Index()
        {
            ViewData["acl-rules"] = _aclService.GetOptions();
            return View();
        }

        [HttpPost]
        public RedirectToActionResult Index(IFormCollection collection)
        {
            var result = _aclService.UpdateAcl(collection);
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