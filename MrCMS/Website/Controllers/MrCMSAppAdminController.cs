using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Controllers
{
    public abstract class MrCMSAppAdminController<T> : MrCMSAdminController //where T : MrCMSApp, new()
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //RouteData.DataTokens["app"] = new T().AppName;
            // TODO: app route data
            base.OnActionExecuting(filterContext);
        }

        protected override void SetDefaultPageTitle(ActionExecutingContext filterContext)
        {
            //ViewBag.Title = string.Format("{0} - {1} - {2}",
            //    new T().AppName.BreakUpString(),
            //    filterContext.RequestContext.RouteData.Values["controller"].ToString()
            //        .BreakUpString(),
            //    filterContext.RequestContext.RouteData.Values["action"].ToString()
            //        .BreakUpString());
            // TODO: default page title
        }
    }
}