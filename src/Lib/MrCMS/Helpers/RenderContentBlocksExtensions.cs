using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.ViewComponents;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers
{
    public static class RenderContentBlocksExtensions
    {
        public static async Task<IHtmlContent> RenderContentBlocks(this IViewComponentHelper helper, Webpage page, IFrontEndEditingChecker roleChecker = null)
        {
            var blocks = page.ContentBlocks.OrderBy(x => x.DisplayOrder);
            IHtmlContentBuilder builder = new HtmlContentBuilder();
            var frontEndEditing = roleChecker != null && await roleChecker.IsAllowed();
            foreach (var block in blocks)
            {
                if (frontEndEditing)
                {
                    builder = builder.AppendHtml($"<div data-content-block-id=\"{block.Id}\">");
                    builder = builder.AppendHtml(
                        $"<div class=\"mrcms-edit-menu mrcms-edit-content-block-area\" data-content-block-menu=\"{block.Id}\"><h4>{block.GetType().Name}</h4><ul><li><a tab-index=\"1\" href=\"/Admin/ContentBlock/Edit/{block.Id}\" class=\"mrcms-btn mrcms-btn-mini mrcms-btn-primary\">Edit</a></li></ul></div>");
                }
                builder = builder.AppendHtml(await helper.InvokeAsync<ContentBlockViewComponent>(new { block }));
                if (frontEndEditing)
                {
                    builder = builder.AppendHtml("</div>");
                }

            }

            return builder;
        }
    }
}