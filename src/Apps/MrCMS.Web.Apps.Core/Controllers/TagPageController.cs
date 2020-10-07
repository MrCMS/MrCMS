using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TagPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly ITagPageUIService _uiService;

        public TagPageController(ITagPageUIService uiService)
        {
            _uiService = uiService;
        }

        [CanonicalLinks("paged-articles")]
        public ViewResult Show(int id, TagPageSearchModel model)
        {
            var page = _uiService.GetPage(id);
            ViewData["paged-articles"] = _uiService.GetWebpages(page, model);
            return View(page);
        }
    }
}