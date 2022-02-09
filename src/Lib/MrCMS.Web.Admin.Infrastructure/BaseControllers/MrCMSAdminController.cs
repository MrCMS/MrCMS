using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Infrastructure.BaseControllers
{
    [Area("Admin")]
    [IgnoreAntiforgeryToken]
    public abstract class MrCMSAdminController : MrCMSController
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // HTTP POST
            // if model state is invalid and it's a post
            if (!ModelState.IsValid &&
                filterContext.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                var modelErrors = ModelState.Values.Where(v => v.Errors.Any()).SelectMany(v => v.Errors);
                var builder = new StringBuilder();
                builder.Append("<p>Validation Errors occurred posting your form.</p>");
                builder.Append("<ul>");
                foreach (var error in modelErrors)
                {
                    builder.Append("<li>");
                    builder.Append(error.ErrorMessage);
                    builder.Append("</li>");
                }

                builder.Append("</ul>");

                TempData.AddErrorMessage(builder.ToString());
                filterContext.Result = new RedirectResult(filterContext.HttpContext.Request.Referer());
            }

            SetDefaultPageTitle(filterContext);
            base.OnActionExecuting(filterContext);
        }

        protected virtual void SetDefaultPageTitle(ActionExecutingContext filterContext)
        {
            ViewBag.Title =
                $"{filterContext.RouteData.Values["controller"]?.ToString().BreakUpString()} - {filterContext.RouteData.Values["action"]?.ToString().BreakUpString()}";
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