using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IGetWebpageForPath
    {
        Task<Webpage> GetWebpage(string path);
    }
}