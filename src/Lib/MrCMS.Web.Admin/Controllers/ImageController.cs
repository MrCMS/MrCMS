using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class ImageController : MrCMSAdminController
    {
        private readonly IImageRenderingService _imageRenderingService;

        public ImageController(IImageRenderingService imageRenderingService)
        {
            _imageRenderingService = imageRenderingService;
        }

        public async Task<JsonResult> GetImageData(string url)
        {
            var imageInfo =
                await _imageRenderingService.GetImageInfo(url,
                    ImageProcessor.GetRequestedSize(url).GetValueOrDefault());

            return imageInfo != null
                ? Json(new {alt = imageInfo.Title, title = imageInfo.Description, url = imageInfo.ImageUrl})
                : Json(new {alt = "", title = ""});
        }
    }
}