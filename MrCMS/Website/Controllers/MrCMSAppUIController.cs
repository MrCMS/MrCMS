
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSAppUIController<T> : MrCMSUIController //where T : MrCMSApp, new()
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: apps
            //RouteData.DataTokens["app"] = new T().AppName;
            base.OnActionExecuting(filterContext);
        }
    }
}