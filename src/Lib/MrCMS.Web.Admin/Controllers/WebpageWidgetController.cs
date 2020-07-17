using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageWidgetController : MrCMSAdminController
    {
        private readonly IWebpageWidgetAdminService _webpageWidgetAdminService;

        public WebpageWidgetController(IWebpageWidgetAdminService webpageWidgetAdminService)
        {
            _webpageWidgetAdminService = webpageWidgetAdminService;
        }

        [HttpPost]
        public RedirectToActionResult Hide(int id, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Hide(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }

        [HttpPost]
        public RedirectToActionResult Show(int id, int widgetId, int layoutAreaId)
        {
            _webpageWidgetAdminService.Show(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }
    }
}