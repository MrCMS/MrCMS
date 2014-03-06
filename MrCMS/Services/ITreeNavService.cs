using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ITreeNavService
    {
        AdminTree GetWebpageNodes(int? id);
        AdminTree GetMediaCategoryNodes(int? id);
        AdminTree GetLayoutNodes(int? id);
    }
}