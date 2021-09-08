using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ITreeNavService
    {
        Task<AdminTree> GetWebpageNodes(int? id);
        Task<bool> WebpageHasChildren(int id);

        Task<AdminTree> GetMediaCategoryNodes(int? id);
        Task<AdminTree> GetLayoutNodes(int? id);
    }
}