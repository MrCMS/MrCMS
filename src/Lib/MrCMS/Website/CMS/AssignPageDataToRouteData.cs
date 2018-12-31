using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public class AssignPageDataToRouteData : IAssignPageDataToRouteData
    {
        public void Assign(RouteData routeData, PageData pageData)
        {
            routeData.Values["id"] = pageData.Id;
            routeData.Values["controller"] = pageData.Controller;
            routeData.Values["action"] = pageData.Action;
            routeData.Values["type"] = pageData.Type;
        }
    }
}