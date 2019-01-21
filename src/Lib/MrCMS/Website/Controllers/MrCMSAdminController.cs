using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Helpers;
using MrCMS.Settings;
using System;

namespace MrCMS.Website.Controllers
{
    //[MrCMSAuthorize]
    //[ValidateInput(false)]
    [Area("Admin")]
    public abstract class MrCMSAdminController : MrCMSController
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // HTTP GET
            // if it's a GET
            if (filterContext.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                // merge in any invalid modelstate found
                if (TempData["MrCMS-invalid-modelstate"] is ModelStateDictionary modelStateDictionary)
                {
                    ModelState.Merge(modelStateDictionary);
                }
            }

            // HTTP POST
            // if model state is invalid and it's a post
            if (!ModelState.IsValid &&
                filterContext.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                // persist the model state to the tempdata dictionary
                TempData["MrCMS-invalid-modelstate"] = ModelState;
                // redirect to the previous page
                filterContext.Result = new RedirectResult(Request.Referer());
            }

            var url = Request.GetDisplayUrl();
            if (filterContext.HttpContext.RequestServices.GetRequiredService<SiteSettings>().SSLAdmin &&
                url.ToLower().Contains("/admin"))
            {
                if (Request.Scheme != "https" && !Request.IsLocal())
                {
                    filterContext.Result = new RedirectResult(url.Replace("http://", "https://"));
                }
            }

            SetDefaultPageTitle(filterContext);
            base.OnActionExecuting(filterContext);
        }

        protected virtual void SetDefaultPageTitle(ActionExecutingContext filterContext)
        {
            ViewBag.Title = $"{filterContext.RouteData.Values["controller"].ToString().BreakUpString()} - {filterContext.RouteData.Values["action"].ToString().BreakUpString()}";
        }

        protected override RedirectResult AuthenticationFailureRedirect()
        {
            return new RedirectResult("~/admin");
        }


        protected void SetSuccessMessage(string message)
        {
            TempData["success-message"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["error-message"] = message;
        }

        protected void SetInfoMessage(string message)
        {
            TempData["info-message"] = message;
        }
    }
}