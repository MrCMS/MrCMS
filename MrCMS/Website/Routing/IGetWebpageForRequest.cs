using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public interface IGetWebpageForRequest
    {
        Webpage Get(RequestContext context);
    }
}