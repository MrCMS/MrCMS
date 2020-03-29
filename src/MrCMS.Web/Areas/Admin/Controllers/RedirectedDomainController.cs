using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class RedirectedDomainController : MrCMSAdminController
    {
        private readonly IRedirectedDomainService _redirectedDomainService;

        public RedirectedDomainController(IRedirectedDomainService redirectedDomainService)
        {
            _redirectedDomainService = redirectedDomainService;
        }

        [HttpGet]
        public PartialViewResult Add(Site site)
        {
            return PartialView(new RedirectedDomain { Site = site });
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(string url, int siteId)
        {
            var rd = new RedirectedDomain { SiteId = siteId, Url = url };

            await _redirectedDomainService.Save(rd);
            return RedirectToAction("Edit", "Sites", new { id = rd.Site.Id });
        }

        public PartialViewResult Delete(RedirectedDomain domain)
        {
            return PartialView(domain);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<RedirectToActionResult> Delete_POST(int id, int siteId)
        {
            await _redirectedDomainService.Delete(id);
            return RedirectToAction("Edit", "Sites", new { id = siteId });
        }
    }
}