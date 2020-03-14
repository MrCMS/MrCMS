using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetCurrentLayout
    {
        Task<Layout> Get(Webpage webpage);
    }
}