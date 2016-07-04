using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities;
using MrCMS.Website.ActionResults;
using MrCMS.Website.Filters;
using StackExchange.Profiling;

namespace MrCMS.Website.Controllers
{
    [ReturnUrlHandler(Order = 999)]
    public abstract class MrCMSController : Controller
    {
        protected IDisposable ActionProfilerStep;
        protected IDisposable ResultProfilerStep;
        protected IDisposable EndToEndProfilerStep;
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

        public new HttpServerUtilityBase Server
        {
            get { return ServerMock ?? base.Server; }
        }

        public HttpServerUtilityBase ServerMock { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            EndToEndProfilerStep =
                GetEndToEndStep(filterContext);
            ActionProfilerStep =
                GetActionStep(filterContext);
            CheckCurrentSite(filterContext);
            base.OnActionExecuting(filterContext);
        }

        protected virtual IDisposable GetActionStep(ActionExecutingContext filterContext)
        {
            return MiniProfiler.Current.Step(string.Format("Executing action {0}/{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName));
        }

        protected virtual IDisposable GetEndToEndStep(ActionExecutingContext filterContext)
        {
            return MiniProfiler.Current.Step(string.Format("End-to-end for {0}/{1}",
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName));
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (ActionProfilerStep != null)
                ActionProfilerStep.Dispose();
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ResultProfilerStep = GetResultStep(filterContext);
        }

        protected virtual IDisposable GetResultStep(ResultExecutingContext filterContext)
        {
            return MiniProfiler.Current.Step(("Executing result"));
        }

        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (ResultProfilerStep != null)
                ResultProfilerStep.Dispose();
            if (EndToEndProfilerStep != null)
                EndToEndProfilerStep.Dispose();
        }

        private void CheckCurrentSite(ActionExecutingContext filterContext)
        {
            List<SiteEntity> entities = filterContext.ActionParameters.Values.OfType<SiteEntity>().ToList();

            if (entities.Any(entity => !CurrentRequestData.CurrentSite.IsValidForSite(entity) && entity.Id != 0) ||
                entities.Any(entity => entity.IsDeleted))
            {
                if (!filterContext.IsChildAction)
                    filterContext.Result = AuthenticationFailureRedirect();
            }
        }

        protected virtual RedirectResult AuthenticationFailureRedirect()
        {
            return Redirect("~");
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }
}