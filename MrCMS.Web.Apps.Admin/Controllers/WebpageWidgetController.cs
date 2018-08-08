using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WebpageWidgetController : MrCMSAdminController
    {
        private readonly IWebpageWidgetAdminService _webpageWidgetAdminService;

        public WebpageWidgetController(IWebpageWidgetAdminService webpageWidgetAdminService)
        {
            _webpageWidgetAdminService = webpageWidgetAdminService;
        }

        [HttpPost]
        public ActionResult Hide(int id, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Hide(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }

        [HttpPost]
        public ActionResult Show(int id, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Show(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }
    }
}