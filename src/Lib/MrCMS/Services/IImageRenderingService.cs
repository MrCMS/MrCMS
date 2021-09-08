using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IImageRenderingService
    {
        Task<IHtmlContent> RenderImage(IHtmlHelper helper, string imageUrl, Size targetSize = default(Size),
            string alt = null, string title = null, object attributes = null);

        Task<string> GetImageUrl(string imageUrl, Size targetSize);
        Task<ImageInfo> GetImageInfo(string imageUrl, Size targetSize);
    }
}