using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website.Auth;

namespace MrCMS.Helpers;

public static class RenderContentExtensions
{
    public static async Task RenderContent(this IHtmlHelper helper, Webpage webpage)
    {
        var activeContent = await GetPreview(helper, webpage) ?? GetLive(webpage);
        if (activeContent == null)
            return;

        if (activeContent.IsDraft)
        {
            await helper.RenderPartialAsync("_PageContentPreview", activeContent);
        }
        else
        {
            await helper.RenderPartialAsync("_PageContent", activeContent);
        }
    }


    private static async Task<ContentVersion> GetPreview(IHtmlHelper helper, Webpage webpage)
    {
        var version = helper.ViewContext.HttpContext.Request.Query["version"];
        if (!int.TryParse(version, out var previewId))
        {
            return null;
        }

        // todo: replace with ACL?
        var user = await helper.GetRequiredService<IGetCurrentUser>().Get();
        if (user?.IsAdmin != true)
            return null;
        return webpage.ContentVersions.FirstOrDefault(x => x.Id == previewId);
    }

    private static ContentVersion GetLive(Webpage webpage)
    {
        return webpage.LiveContentVersion;
    }
}