using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MrCMS.Entities;
using MrCMS.Website.Filters;

namespace MrCMS.Website.Controllers
{
    [ReturnUrlHandler(Order = 999)]
    public abstract class MrCMSController : Controller
    {
        // TODO: profiler
        //protected IDisposable ActionProfilerStep;
        //protected IDisposable ResultProfilerStep;
        //protected IDisposable EndToEndProfilerStep;

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    EndToEndProfilerStep =
        //        GetEndToEndStep(filterContext);
        //    ActionProfilerStep =
        //        GetActionStep(filterContext);
        //    CheckCurrentSite(filterContext);
        //    base.OnActionExecuting(filterContext);
        //}

        //protected virtual IDisposable GetActionStep(ActionExecutingContext filterContext)
        //{
        //    return MiniProfiler.Current.Step(string.Format("Executing action {0}/{1}",
        //        filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
        //        filterContext.ActionDescriptor.ActionName));
        //}

        //protected virtual IDisposable GetEndToEndStep(ActionExecutingContext filterContext)
        //{
        //    return MiniProfiler.Current.Step(string.Format("End-to-end for {0}/{1}",
        //        filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
        //        filterContext.ActionDescriptor.ActionName));
        //}

        //public override void OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if (ActionProfilerStep != null)
        //        ActionProfilerStep.Dispose();
        //}

        //protected override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    ResultProfilerStep = GetResultStep(filterContext);
        //}

        //protected virtual IDisposable GetResultStep(ResultExecutingContext filterContext)
        //{
        //    return MiniProfiler.Current.Step(("Executing result"));
        //}

        //protected override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    if (ResultProfilerStep != null)
        //        ResultProfilerStep.Dispose();
        //    if (EndToEndProfilerStep != null)
        //        EndToEndProfilerStep.Dispose();
        //}

        //private void CheckCurrentSite(ActionExecutingContext filterContext)
        //{
        //    List<SiteEntity> entities = filterContext.ActionParameters.Values.OfType<SiteEntity>().ToList();

        //    if (entities.Any(entity => !CurrentRequestData.CurrentSite.IsValidForSite(entity) && entity.Id != 0) ||
        //        entities.Any(entity => entity.IsDeleted))
        //    {
        //        if (!filterContext.IsChildAction)
        //            filterContext.Result = AuthenticationFailureRedirect();
        //    }
        //}

        protected virtual RedirectResult AuthenticationFailureRedirect()
        {
            return Redirect("~");
        }

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
        //    JsonRequestBehavior behavior)
        //{
        //    return new JsonNetResult
        //    {
        //        ContentEncoding = contentEncoding,
        //        ContentType = contentType,
        //        Data = data,
        //        JsonRequestBehavior = behavior
        //    };
        //}
    }
}