using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaSelectorController : MrCMSAdminController
    {
        private readonly IMediaSelectorService _mediaSelectorService;

        public MediaSelectorController(IMediaSelectorService mediaSelectorService)
        {
            _mediaSelectorService = mediaSelectorService;
        }

        public ActionResult Show(MediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = _mediaSelectorService.GetCategories();
            ViewData["results"] = _mediaSelectorService.Search(searchQuery);
            return PartialView(searchQuery);
        }
        public ActionResult CKEditor(CKEditorMediaSelectorSearchQuery searchQuery)
        {
            ViewData["categories"] = _mediaSelectorService.GetCategories();
            ViewData["results"] = _mediaSelectorService.Search(searchQuery);
            return View(searchQuery);
        }

        [HttpGet]
        public async Task<JsonResult> Alt(string url)
        {
            return Json(new { alt = await _mediaSelectorService.GetAlt(url) });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAlt(UpdateMediaParams updateMediaParams)
        {
            return Json(await _mediaSelectorService.UpdateAlt(updateMediaParams));
        }

        [HttpGet]
        public async Task<JsonResult> Description(string url)
        {
            return Json(new { description = await _mediaSelectorService.GetDescription(url) });
        }

        [HttpPost]
        public async Task<JsonResult> UpdateDescription(UpdateMediaParams updateMediaParams)
        {
            return Json(await _mediaSelectorService.UpdateDescription(updateMediaParams));
        }

        public async Task<JsonResult> GetFileInfo(string value)
        {
            return Json(await _mediaSelectorService.GetFileInfo(value));
        }
    }
}