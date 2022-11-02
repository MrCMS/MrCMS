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
        public static async Task<IHtmlContent> RenderImage(this IHtmlHelper helper, string imageUrl,
            Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null, bool enableCaption = false,
            bool showPlaceHolderIfNull = false, bool enableLazyLoading = true)
        {
            return await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .RenderImage(helper, imageUrl, targetSize, alt, title, enableCaption, attributes, showPlaceHolderIfNull,
                    enableLazyLoading);
        }

        public static async Task<string> GetImageUrl(this IHtmlHelper helper, string imageUrl,
            Size targetSize = default(Size))
        {
            return await helper.ViewContext.HttpContext.RequestServices.GetRequiredService<IImageRenderingService>()
                .GetImageUrl(imageUrl, targetSize);
        }
    }
}