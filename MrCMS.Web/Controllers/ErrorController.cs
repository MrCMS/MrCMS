using System.Web.Mvc;
using MrCMS.Services;

namespace MrCMS.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly IDocumentService _documentService;

        public ErrorController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [ActionName("404")]
        public ActionResult Error404()
        {
            var page = _documentService.Get404Page();
            return View("~/Views/TextPage/Show.cshtml", page.CurrentLayout.UrlSegment, page);
        }

        [ActionName("500")]
        public ActionResult Error500()
        {
            var page = _documentService.Get500Page();
            return View("~/Views/TextPage/Show.cshtml", page.CurrentLayout.UrlSegment, page);
        }
    }
}