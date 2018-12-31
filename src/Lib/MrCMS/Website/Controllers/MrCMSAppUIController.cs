
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Apps;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSAppUIController<T> : MrCMSUIController where T : IMrCMSApp, new()
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RouteData.DataTokens["app"] = new T().Name;
            base.OnActionExecuting(filterContext);
        }
    }
}