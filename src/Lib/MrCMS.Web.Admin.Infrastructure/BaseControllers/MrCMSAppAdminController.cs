using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Apps;
using MrCMS.Helpers;

namespace MrCMS.Web.Admin.Infrastructure.BaseControllers
{
    public abstract class MrCMSAppAdminController<T> : MrCMSAdminController where T : IMrCMSApp, new()
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RouteData.DataTokens["app"] = new T().Name;
            base.OnActionExecuting(filterContext);
        }

        protected override void SetDefaultPageTitle(ActionExecutingContext filterContext)
        {
            ViewBag.Title = string.Format("{0} - {1} - {2}",
                new T().Name.BreakUpString(),
                filterContext.RouteData.Values["controller"].ToString()
                    .BreakUpString(),
                filterContext.RouteData.Values["action"].ToString()
                    .BreakUpString());
        }
    }
}