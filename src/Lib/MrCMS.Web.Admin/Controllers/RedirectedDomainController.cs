using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Admin.Controllers
{
    public class RedirectedDomainController : MrCMSAdminController
    {
        private readonly IRedirectedDomainService _redirectedDomainService;
        private readonly ISession _session;

        public RedirectedDomainController(IRedirectedDomainService redirectedDomainService, ISession session)
        {
            _redirectedDomainService = redirectedDomainService;
            _session = session;
        }

        [HttpGet]
        public PartialViewResult Add(Site site)
        {
            return PartialView(new RedirectedDomain {Site = site});
        }

        [HttpPost]
        public RedirectToActionResult Add(string url, int siteId)
        {
            var site = _session.Get<Site>(siteId);
            var rd = new RedirectedDomain(){Site = site, Url = url};
            
            _redirectedDomainService.Save(rd);
            return RedirectToAction("Edit", "Sites", new {id = rd.Site.Id});
        }

        public PartialViewResult Delete(RedirectedDomain domain)
        {
            return PartialView(domain);
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToActionResult Delete_POST(int id, int siteId)
        {
            _redirectedDomainService.Delete(id);
            return RedirectToAction("Edit", "Sites", new {id = siteId});
        }
    }
}