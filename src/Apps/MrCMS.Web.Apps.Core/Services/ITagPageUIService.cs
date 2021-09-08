using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface ITagPageUIService
    {
        Task<TagPage> GetPage(int id);
    }
}