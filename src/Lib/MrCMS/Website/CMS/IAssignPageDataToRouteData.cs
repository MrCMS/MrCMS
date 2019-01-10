using Microsoft.AspNetCore.Routing;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface IAssignPageDataToRouteData
    {
        void Assign(RouteData routeData, PageData pageData);
    }
}