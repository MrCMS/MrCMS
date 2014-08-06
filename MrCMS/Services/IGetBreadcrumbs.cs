using System.Collections.Generic;
using MrCMS.Entities.Documents;

namespace MrCMS.Services
{
    public interface IGetBreadcrumbs
    {
        IEnumerable<Document> Get(int? parent);
    }
}