using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class MediaSelectorController : MrCMSAdminController
    {
        private readonly IMediaSelectorService _mediaSelectorService;

        public MediaSelectorController(IMediaSelectorService mediaSelectorService)
        {
            _mediaSelectorService = mediaSelectorService;
        }

        public async Task<ActionResult> Show(MediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = await _mediaSelectorService.GetCategories();
            ViewData["results"] = await _mediaSelectorService.Search(searchQuery);
            return PartialView(searchQuery);
        }
        public async Task<ActionResult> CKEditor(CKEditorMediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = await _mediaSelectorService.GetCategories();
            ViewData["results"] = await _mediaSelectorService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public async Task<JsonResult> Alt(string url)
        {
            return Json(new { alt = await _mediaSelectorService.GetAlt(url) });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAlt(UpdateMediaParams updateMediaParams, CancellationToken token)
        {
            return Json(await _mediaSelectorService.UpdateAlt(updateMediaParams, token));
        }

        [HttpGet]
        public async Task<JsonResult> Description(string url)
        {
            return Json(new { description = await _mediaSelectorService.GetDescription(url) });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateDescription(UpdateMediaParams updateMediaParams, CancellationToken token)
        {
            return Json(await _mediaSelectorService.UpdateDescription(updateMediaParams, token));
        }

        public async Task<JsonResult> GetFileInfo(string value)
        {
            return Json(await _mediaSelectorService.GetFileInfo(value));
        }
    }
}