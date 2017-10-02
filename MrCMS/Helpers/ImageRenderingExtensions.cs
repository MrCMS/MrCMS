using System.Drawing;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class ImageRenderingExtensions
    {
        public static MvcHtmlString RenderImage(this HtmlHelper helper, string imageUrl, Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null)
        {
            return helper.Get<IImageRenderingService>()
                .RenderImage(helper, imageUrl, targetSize, alt, title, attributes);
        }
        public static string GetImageUrl(this HtmlHelper helper, string imageUrl, Size targetSize = default(Size))
        {
            return helper.Get<IImageRenderingService>()
                .GetImageUrl(imageUrl, targetSize);
        }
    }
}