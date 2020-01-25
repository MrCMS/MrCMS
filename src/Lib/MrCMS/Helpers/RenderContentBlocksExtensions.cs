using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.ViewComponents;

namespace MrCMS.Helpers
{
    public static class RenderContentBlocksExtensions
    {
        public static async Task<IHtmlContent> RenderContentBlocks(this IViewComponentHelper helper, Webpage page)
        {
            return await helper.InvokeAsync<ContentBlocksViewComponent>(new {page.Id});
        }
    }
}