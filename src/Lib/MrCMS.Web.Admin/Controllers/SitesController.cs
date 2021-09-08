using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Models;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.ModelBinders;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class SitesController : MrCMSAdminController
    {
        private readonly ISiteAdminService _siteAdminService;

        public SitesController(ISiteAdminService siteAdminService)
        {
            _siteAdminService = siteAdminService;
        }

        [HttpGet]
        [ActionName("Index")]
        public async Task<ViewResult> Index_Get()
        {
            var sites = await _siteAdminService.GetAllSites();
            return View("Index", sites);
        }

        [HttpGet]
        [ActionName("Add")]
        public ViewResult Add_Get()
        {
            return View();
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddSiteModel model,
            [ModelBinder(typeof(GetSiteCopyOptionsModelBinder))]
            List<SiteCopyOption> options)
        {
            await _siteAdminService.AddSite(model, options);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<ViewResult> Edit_Get(int id)
        {
            return View(await _siteAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateSiteModel model)
        {
            await _siteAdminService.SaveSite(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<PartialViewResult> Delete_Get(int id)
        {
            return PartialView(await _siteAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Delete(int id)
        {
            await _siteAdminService.DeleteSite(id);
            return RedirectToAction("Index");
        }
    }
}