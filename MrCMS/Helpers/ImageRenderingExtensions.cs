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
            return MrCMSApplication.Get<IImageTagRenderingService>()
                .RenderImage(helper, imageUrl, targetSize, alt, title, attributes);
        }
    }
}