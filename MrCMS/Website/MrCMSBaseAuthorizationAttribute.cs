using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Website.Routing;

namespace MrCMS.Website
{
    public abstract class MrCMSBaseAuthorizationAttribute : AuthorizeAttribute
    {
        public bool ReturnEmptyResult { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (ReturnEmptyResult)
            {
                filterContext.Result = new EmptyResult();
            }
            else
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
}