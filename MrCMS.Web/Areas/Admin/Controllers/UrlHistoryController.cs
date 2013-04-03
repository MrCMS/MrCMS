using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UrlHistoryController : MrCMSAdminController
    {
        private readonly IUrlHistoryService _urlHistoryService;
        private readonly IDocumentService _documentService;

        public UrlHistoryController(IUrlHistoryService urlHistoryService, IDocumentService documentService)
        {
            _urlHistoryService = urlHistoryService;
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
            _urlHistoryService.Delete(history);

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
            _urlHistoryService.Add(history);

            return RedirectToAction("Edit", "Webpage", new { id = history.Webpage.Id });
        }

        public ActionResult ValidateUrlIsAllowed(string urlsegment)
        {
            return !_documentService.UrlIsValidForWebpageUrlHistory(urlsegment) ? Json("Please choose a different URL as this one is already used.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
