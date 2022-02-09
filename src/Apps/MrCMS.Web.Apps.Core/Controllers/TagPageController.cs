using System.Threading.Tasks;
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
        public async Task<ViewResult> Show(int id, TagPageSearchModel model)
        {
            var page = await _uiService.GetPage(id);
            ViewData["search-model"] = model;
            return View(page);
        }
    }
}