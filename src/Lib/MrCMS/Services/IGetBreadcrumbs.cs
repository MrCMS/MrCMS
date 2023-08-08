using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IGetBreadcrumbs
    {
        Task<IReadOnlyList<Webpage>> Get(int? parent);
    }
}