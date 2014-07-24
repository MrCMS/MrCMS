using System.Collections.Specialized;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.Routing
{
    public interface IControllerManager
    {
        IControllerFactory OverridenControllerFactory { get; set; }
        IControllerFactory ControllerFactory { get; }
        void SetFormData(Webpage webpage, Controller controller, NameValueCollection form);
        string GetActionName(Webpage webpage, string httpMethod);
        Controller GetController(RequestContext requestContext, Webpage webpage, string httpMethod);
        string GetControllerName(Webpage webpage, string httpMethod);
    }
}