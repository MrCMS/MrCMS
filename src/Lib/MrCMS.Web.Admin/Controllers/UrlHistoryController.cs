using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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
        public ActionResult Delete(int id)
        {
            var history =_urlHistoryAdminService.Delete(id);

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
        public ActionResult Add(AddUrlHistoryModel model)
        {
            _urlHistoryAdminService.Add(model);

            return RedirectToAction("Edit", "Webpage", new { id = model.WebpageId });
        }

        public ActionResult ValidateUrlIsAllowed(string urlsegment)
        {
            return !_urlValidationService.UrlIsValidForWebpageUrlHistory(urlsegment)
                ? Json("Please choose a different URL as this one is already used.")
                : Json(true);
        }
    }
}
