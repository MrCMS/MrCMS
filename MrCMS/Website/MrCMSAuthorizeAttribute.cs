using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Website.Routing;
using Castle.Core.Internal;

namespace MrCMS.Website
{
    public class MrCMSAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext) && CurrentRequestData.CurrentUser != null &&
                   CurrentRequestData.CurrentUser.IsActive &&
                   CurrentRequestData.CurrentUser.Email == httpContext.User.Identity.Name;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.Controller.GetType().GetCustomAttributes(typeof (MrCMSAuthorizeAttribute), true).Any())
            {
                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    var mrCMSHttpHandler = MrCMSApplication.Get<MrCMSHttpHandler>();
                    var routeData = filterContext.RouteData;
                    routeData.Route = RouteTable.Routes.Last();
                    routeData.DataTokens.Remove("area");
                    mrCMSHttpHandler.SetRequestContext(new RequestContext(filterContext.HttpContext,
                                                                          routeData));
                    mrCMSHttpHandler.Handle403(filterContext.HttpContext);
                    filterContext.Result = new EmptyResult();
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
            }
        }
    }
}