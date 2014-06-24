using System.Web.Mvc;
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
            return PartialView(new RedirectedDomain {Site = site});
        }

        [HttpPost]
        public RedirectToRouteResult Add(RedirectedDomain domain)
        {
            _redirectedDomainService.Save(domain);
            return RedirectToAction("Edit", "Sites", new {id = domain.Site.Id});
        }

        public PartialViewResult Delete(RedirectedDomain domain)
        {
            return PartialView(domain);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToRouteResult Delete_POST(RedirectedDomain domain)
        {
            Site site = domain.Site;
            _redirectedDomainService.Delete(domain);
            return RedirectToAction("Edit", "Sites", new {id = site.Id});
        }
    }
}