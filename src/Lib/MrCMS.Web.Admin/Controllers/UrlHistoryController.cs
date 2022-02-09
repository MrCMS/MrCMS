using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class UrlHistoryController : MrCMSAdminController
    {
        private readonly IUrlHistoryAdminService _urlHistoryAdminService;
        private readonly IUrlValidationService _urlValidationService;
        private readonly ICurrentSiteLocator _currentSiteLocator;

        public UrlHistoryController(IUrlHistoryAdminService urlHistoryAdminService,
            IUrlValidationService urlValidationService, ICurrentSiteLocator currentSiteLocator)
        {
            _urlHistoryAdminService = urlHistoryAdminService;
            _urlValidationService = urlValidationService;
            _currentSiteLocator = currentSiteLocator;
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete_Get(int id)
        {
            return View(await _urlValidationService.Get(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var history = await _urlHistoryAdminService.Delete(id);

            return RedirectToAction("Edit", "Webpage", new { id = history.Webpage.Id });
        }

        [HttpGet]
        [ActionName("Add")]
        public ActionResult Add_Get(int webpageId)
        {
            var urlHistory = _urlHistoryAdminService.GetUrlHistoryToAdd(webpageId);

            return View(urlHistory);
        }

        [HttpPost]
        public async Task<ActionResult> Add(AddUrlHistoryModel model)
        {
            await _urlHistoryAdminService.Add(model);

            return RedirectToAction("Edit", "Webpage", new { id = model.WebpageId });
        }

        public async Task<ActionResult> ValidateUrlIsAllowed(string urlsegment)
        {
            return !await _urlValidationService.UrlIsValidForWebpageUrlHistory(_currentSiteLocator.GetCurrentSite().Id,
                urlsegment)
                ? Json("Please choose a different URL as this one is already used.")
                : Json(true);
        }
    }
}