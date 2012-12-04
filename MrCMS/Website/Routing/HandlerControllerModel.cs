using System.Web.Mvc;
using System.Web.Routing;

namespace MrCMS.Website.Routing
{
    public class HandlerControllerModel
    {
        public HandlerControllerModel(Controller controller, RouteValueDictionary routeData)
        {
            Controller = controller;
            RouteData = routeData;
        }

        public Controller Controller { get; set; }
        public RouteValueDictionary RouteData { get; set; }
    }
}