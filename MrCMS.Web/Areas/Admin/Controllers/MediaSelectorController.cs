using System.Web.Mvc;
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


        public JsonResult GetFileInfo(string value)
        {
            return Json(_mediaSelectorService.GetFileInfo(value), JsonRequestBehavior.AllowGet);
        }
    }
}