using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ITreeNavService
    {
        AdminTree GetWebpageNodes(int? id);
        AdminTree GetMediaCategoryNodes(int? id);
        AdminTree GetLayoutNodes(int? id);
    }
}