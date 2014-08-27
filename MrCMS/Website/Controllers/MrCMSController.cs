using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities;
using MrCMS.Website.Filters;

namespace MrCMS.Website.Controllers
{
    [ReturnUrlHandler(Order = 999)]
    public abstract class MrCMSController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CheckCurrentSite(filterContext);
            base.OnActionExecuting(filterContext);
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            var entities = filterContext.ActionParameters.Values.OfType<SiteEntity>().ToList();

            if (entities.Any(entity => !CurrentRequestData.CurrentSite.IsValidForSite(entity) && entity.Id != 0) || entities.Any(entity => entity.IsDeleted))
            {
                filterContext.Result = AuthenticationFailureRedirect();
            }
        }
        public string ReferrerOverride { get; set; }
        protected Uri Referrer
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ReferrerOverride)
                           ? new Uri(ReferrerOverride)
                           : HttpContext.Request.UrlReferrer;
            }
        }


        public new HttpRequestBase Request
        {
            get { return RequestMock ?? base.Request; }
        }

        public new RouteData RouteData
        {
            get { return RouteDataMock ?? base.RouteData; }
        }

        public HttpRequestBase RequestMock { get; set; }
        public RouteData RouteDataMock { get; set; }

        protected virtual RedirectResult AuthenticationFailureRedirect()
        {
            return Redirect("~");
        }

        public new HttpServerUtilityBase Server
        {
            get { return ServerMock ?? base.Server; }
        }

        public HttpServerUtilityBase ServerMock { get; set; }
    }
}