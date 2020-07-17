using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ITreeNavService
    {
        AdminTree GetWebpageNodes(int? id);
        bool WebpageHasChildren(int id);

        AdminTree GetMediaCategoryNodes(int? id);
        AdminTree GetLayoutNodes(int? id);
    }
}