using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<RedirectToActionResult> Hide(int id, int widgetId, int layoutAreaId)
        {
            await _webpageWidgetAdminService.Hide(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Show(int id, int widgetId, int layoutAreaId)
        {
            await _webpageWidgetAdminService.Show(id, widgetId);
            return RedirectToAction("Edit", "Webpage", new { id, layoutAreaId });
        }
    }
}