using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class ImageController : MrCMSAdminController
    {
        private readonly IImageProcessor _imageProcessor;

        public ImageController(IImageProcessor imageProcessor)
        {
            _imageProcessor = imageProcessor;
        }

        public JsonResult GetImageData(string url)
        {
            var mediaFile = _imageProcessor.GetImage(url);

            return mediaFile != null
                       ? Json(new {alt = mediaFile.Title, title = mediaFile.Description})
                       : Json(new {alt = "", title = ""});
        }
    }
}