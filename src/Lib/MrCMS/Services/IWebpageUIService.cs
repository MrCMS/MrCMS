using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IWebpageUIService
    {
        Task<T> GetPage<T>(int id) where T : Webpage;
    }
}