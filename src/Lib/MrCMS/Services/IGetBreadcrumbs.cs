using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetBreadcrumbs
    {
        Task<IReadOnlyList<Document>> Get(int? parent);
    }
}