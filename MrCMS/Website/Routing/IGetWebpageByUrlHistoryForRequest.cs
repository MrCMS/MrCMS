using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public interface IGetWebpageByUrlHistoryForRequest
    {
        Webpage Get(RequestContext context);
    }
}