using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetWebpageByUrl<T> where T : Webpage
    {
        Task<T> GetByUrl(string url);
    }
}