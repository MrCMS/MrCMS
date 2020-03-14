using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        private readonly IAclAdminService _aclService;
        private readonly IConfigurationProvider _configurationProvider;

        public ACLController(IAclAdminService aclService, IConfigurationProvider configurationProvider)
        {
            _aclService = aclService;
            _configurationProvider = configurationProvider;
        }

        public ViewResult Index()
        {
            ViewData["acl-rules"] = _aclService.GetOptions();
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Index(IFormCollection collection)
        {
            await _aclService.UpdateAcl(collection);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public async Task<ActionResult> Disable()
        {
            ACLSettings aclSettings = await _configurationProvider.GetSiteSettings<ACLSettings>();
            aclSettings.ACLEnabled = false;
            await _configurationProvider.SaveSettings(aclSettings);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public async Task<ActionResult> Enable()
        {
            ACLSettings aclSettings = await _configurationProvider.GetSiteSettings<ACLSettings>();
            aclSettings.ACLEnabled = true;
            await _configurationProvider.SaveSettings(aclSettings);
            return RedirectToAction("Index");
        }
    }
}