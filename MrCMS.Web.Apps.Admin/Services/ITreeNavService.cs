using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ITreeNavService
    {
        AdminTree GetWebpageNodes(int? id);
        bool WebpageHasChildren(int id);

        AdminTree GetMediaCategoryNodes(int? id);
        AdminTree GetLayoutNodes(int? id);
    }
}