using System.Web.Mvc;
using MrCMS.Apps;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSAppUIController<T> : MrCMSUIController where T : MrCMSApp, new()
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RouteData.DataTokens["app"] = new T().AppName;
            base.OnActionExecuting(filterContext);
        }
    }
}