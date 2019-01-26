using System.Drawing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IImageRenderingService
    {
        IHtmlContent RenderImage(IHtmlHelper helper, string imageUrl, Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null);

        string GetImageUrl(string imageUrl, Size targetSize);
        ImageInfo GetImageInfo(string imageUrl, Size targetSize);
    }
}