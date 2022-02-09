using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public class AssignPageDataToRouteValues : IAssignPageDataToRouteValues
    {
        public void Assign(RouteData routeData, PageData pageData)
        {
        }

        public void Assign(RouteValueDictionary values, PageData pageData)
        {
            values["id"] = pageData.Id;
            values["controller"] = pageData.Controller;
            values["action"] = pageData.Action;
            values["type"] = pageData.Type;
        }
    }
}