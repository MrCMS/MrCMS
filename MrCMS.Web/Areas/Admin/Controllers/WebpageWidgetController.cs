using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageWidgetController : MrCMSAdminController
    {
        private readonly IWebpageWidgetAdminService _webpageWidgetAdminService;

        public WebpageWidgetController(IWebpageWidgetAdminService webpageWidgetAdminService)
        {
            _webpageWidgetAdminService = webpageWidgetAdminService;
        }

        [HttpPost]
        public ActionResult Hide(Webpage document, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Hide(document, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id = document.Id, layoutAreaId });
        }

        [HttpPost]
        public ActionResult Show(Webpage document, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Show(document, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id = document.Id, layoutAreaId });
        }
    }
}