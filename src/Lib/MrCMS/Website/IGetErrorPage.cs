using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetErrorPage
    {
        Task<Webpage> GetPage(int code);
    }
}