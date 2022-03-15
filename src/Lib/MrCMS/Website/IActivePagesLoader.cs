using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IActivePagesLoader
    {
        Task<List<Webpage>> GetActivePages(Webpage webpage);
    }
}