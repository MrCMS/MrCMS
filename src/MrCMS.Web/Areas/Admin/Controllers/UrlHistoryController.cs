using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UrlHistoryController : MrCMSAdminController
    {
        private readonly IUrlHistoryAdminService _urlHistoryAdminService;
        private readonly IUrlValidationService _urlValidationService;

        public UrlHistoryController(IUrlHistoryAdminService urlHistoryAdminService, IUrlValidationService urlValidationService)
        {
            _urlHistoryAdminService = urlHistoryAdminService;
            _urlValidationService = urlValidationService;
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(UrlHistory history)
        {
            return View(history);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var history = await _urlHistoryAdminService.Delete(id);

            return RedirectToAction("Edit", "Webpage", new { id = history.WebpageId });
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
        await    _urlHistoryAdminService.Add(model);

            return RedirectToAction("Edit", "Webpage", new { id = model.WebpageId });
        }

        public async Task<ActionResult> ValidateUrlIsAllowed(string urlsegment)
        {
            return !await _urlValidationService.UrlIsValidForWebpageUrlHistory(urlsegment)
                ? Json("Please choose a different URL as this one is already used.")
                : Json(true);
        }
    }
}
