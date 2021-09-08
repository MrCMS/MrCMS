using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
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
        public PartialViewResult Add(int Id)
        {
            return PartialView(new AddRedirectedDomainModel {SiteId = Id});
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddRedirectedDomainModel model)
        {
            var site = await _session.GetAsync<Site>(model.SiteId);
            var rd = new RedirectedDomain() {Site = site, Url = model.Url};

            await _redirectedDomainService.Save(rd);
            return RedirectToAction("Edit", "Sites", new {id = model.SiteId});
        }

        public async Task<PartialViewResult> Delete(int id)
        {
            return PartialView(await _session.GetAsync<RedirectedDomain>(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<RedirectToActionResult> Delete_POST(int id, int siteId)
        {
            await _redirectedDomainService.Delete(id);
            return RedirectToAction("Edit", "Sites", new {id = siteId});
        }
    }
}