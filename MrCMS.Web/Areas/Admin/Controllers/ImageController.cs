using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ImageController : MrCMSAdminController
    {
        private readonly IImageRenderingService _imageRenderingService;

        public ImageController(IImageRenderingService imageRenderingService)
        {
            _imageRenderingService = imageRenderingService;
        }

        public JsonResult GetImageData(string url)
        {
            var imageInfo =
                _imageRenderingService.GetImageInfo(url, ImageProcessor.GetRequestedSize(url).GetValueOrDefault());

            return imageInfo != null
                ? Json(new { alt = imageInfo.Title, title = imageInfo.Description, url = imageInfo.ImageUrl })
                : Json(new { alt = "", title = "" });
        }
    }
}