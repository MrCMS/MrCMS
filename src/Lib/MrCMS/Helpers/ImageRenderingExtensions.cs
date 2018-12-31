using System.Drawing;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class ImageRenderingExtensions
    {
        public static IHtmlContent RenderImage(this IHtmlHelper helper, string imageUrl,
            Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .RenderImage(helper, imageUrl, targetSize, alt, title, attributes);
        }

        public static string GetImageUrl(this IHtmlHelper helper, string imageUrl, Size targetSize = default(Size))
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .GetImageUrl(imageUrl, targetSize);
        }
    }
}