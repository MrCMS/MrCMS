using Microsoft.AspNetCore.Routing;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface IAssignPageDataToRouteData
    {
        void Assign(RouteData routeData, PageData pageData);
    }
    public class AssignPageDataToRouteData : IAssignPageDataToRouteData
    {
        //private readonly IRepository<Webpage> _repository;

        //public AssignPageDataToRouteData(IRepository<Webpage> repository)
        //{
        //    _repository = repository;
        //}
        public void Assign(RouteData routeData, PageData pageData)
        {
            routeData.Values["id"] = pageData.Id;
            //routeData.Values["page"] = _repository.Get(pageData.Id);
            routeData.Values["controller"] = pageData.Controller;
            routeData.Values["action"] = pageData.Action;
            routeData.Values["type"] = pageData.Type;
        }
    }

}