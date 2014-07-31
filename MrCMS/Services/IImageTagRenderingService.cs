using System.Drawing;
using System.Web.Mvc;

namespace MrCMS.Services
{
    public interface IImageTagRenderingService
    {
        MvcHtmlString RenderImage(HtmlHelper helper, string imageUrl, Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null);
    }
}