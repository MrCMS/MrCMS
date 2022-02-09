using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website
{
    public interface IActivePagesLoader
    {
        List<Webpage> GetActivePages(Webpage webpage);
    }
}