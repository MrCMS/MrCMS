using Microsoft.AspNetCore.Routing;

namespace MrCMS.Website.CMS
{
    public interface IAssignPageDataToRouteValues
    {
        void Assign(RouteValueDictionary values, PageData pageData);
    }
}