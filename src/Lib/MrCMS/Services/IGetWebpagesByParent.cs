using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetWebpagesByParent
    {
        Task<IReadOnlyList<Webpage>> GetWebpages(Webpage parent);
    }
}