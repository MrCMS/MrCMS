using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetHomePage
    {
        Task<Webpage> Get();
    }
}