using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL;
using MrCMS.ACL.Rules;
using MrCMS.Settings;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
{
    public class ACLController : MrCMSAdminController
    {
        private readonly IAclAdminService _aclService;
        private readonly ACLSettings _aclSettings;
        private readonly IConfigurationProvider _configurationProvider;

        public ACLController(IAclAdminService aclService, ACLSettings aclSettings,
            IConfigurationProvider configurationProvider)
        {
            _aclService = aclService;
            _aclSettings = aclSettings;
            _configurationProvider = configurationProvider;
        }

        public async Task<ViewResult> Index()
        {
            ViewData["acl-rules"] = await _aclService.GetOptions();
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Index(IFormCollection collection)
        {
            var result = await _aclService.UpdateAcl(collection);
            return RedirectToAction("Index");
        }


        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public async Task<ActionResult> Disable()
        {
            _aclSettings.ACLEnabled = false;
            await _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Acl(typeof(AclAdminACL), AclAdminACL.Edit)]
        public async Task<ActionResult> Enable()
        {
            _aclSettings.ACLEnabled = true;
            await _configurationProvider.SaveSettings(_aclSettings);
            return RedirectToAction("Index");
        }
    }
}