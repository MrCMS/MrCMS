using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UrlHistoryController : MrCMSAdminController
    {
        private readonly IUrlHistoryAdminService _urlHistoryAdminService;
        private readonly IDocumentService _documentService;

        public UrlHistoryController(IUrlHistoryAdminService urlHistoryAdminService, IDocumentService documentService)
        {
            _urlHistoryAdminService = urlHistoryAdminService;
            _documentService = documentService;
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(UrlHistory history)
        {
            return View(history);
        }

        [HttpPost]
        public ActionResult Delete(UrlHistory history)
        {
            _urlHistoryAdminService.Delete(history);

            return RedirectToAction("Edit", "Webpage", new { id = history.Webpage.Id });
        }

        [HttpGet]
        [ActionName("Add")]
        public ActionResult Add_Get(int webpageId)
        {
            var urlHistroy = new UrlHistory
                                 {
                                     Webpage = _documentService.GetDocument<Webpage>(webpageId)
                                 };

            return View(urlHistroy);
        }

        [HttpPost]
        public ActionResult Add(UrlHistory history)
        {
            _urlHistoryAdminService.Add(history);

            return RedirectToAction("Edit", "Webpage", new { id = history.Webpage.Id });
        }

        public ActionResult ValidateUrlIsAllowed(string urlsegment)
        {
            return !_documentService.UrlIsValidForWebpageUrlHistory(urlsegment) ? Json("Please choose a different URL as this one is already used.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
