using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;

namespace MrCMS.Helpers
{
    public static class ImageRenderingExtensions
    {
        public static Task<IHtmlContent> RenderImage(this IHtmlHelper helper, string imageUrl,
            Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null, bool enableCaption = false)
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .RenderImage(helper, imageUrl, targetSize, alt, title, enableCaption, attributes);
        }

        public static Task<string> GetImageUrl(this IHtmlHelper helper, string imageUrl, Size targetSize = default(Size))
        {
            return helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .GetImageUrl(imageUrl, targetSize);
        }
    }
}