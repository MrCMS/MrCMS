using System;
using System.Text;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website.ActionResults;

namespace MrCMS.Website.Controllers
{
    [MrCMSAuthorize]
    [ValidateInput(false)]
    public abstract class MrCMSAdminController : MrCMSController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // HTTP GET
            // if it's a GET
            if (filterContext.HttpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                // merge in any invalid modelstate found
                var modelStateDictionary = TempData["MrCMS-invalid-modelstate"] as ModelStateDictionary;
                if (modelStateDictionary != null)
                {
                    ModelState.Merge(modelStateDictionary);
                }
            }

            // HTTP POST
            // if model state is invalid and it's a post
            if (!ModelState.IsValid &&
                filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                // persist the model state to the tempdata dictionary
                TempData["MrCMS-invalid-modelstate"] = ModelState;
                // redirect to the previous page
                filterContext.Result = new RedirectResult(Referrer.ToString());
            }

            string url = Request.Url.ToString();
            if (MrCMSApplication.Get<SiteSettings>().SSLAdmin && url.ToLower().Contains("/admin"))
            {
                if (!Request.IsSecureConnection && !Request.IsLocal)
                {
                    filterContext.Result = new RedirectResult(url.Replace("http://", "https://"));
                }
            }

            SetDefaultPageTitle(filterContext);
            base.OnActionExecuting(filterContext);
        }

        protected virtual void SetDefaultPageTitle(ActionExecutingContext filterContext)
        {
            ViewBag.Title = string.Format("{0} - {1}",
                filterContext.RequestContext.RouteData.Values["controller"].ToString()
                    .BreakUpString(),
                filterContext.RequestContext.RouteData.Values["action"].ToString()
                    .BreakUpString());
        }

        protected override RedirectResult AuthenticationFailureRedirect()
        {
            return new RedirectResult("~/admin");
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding,
            JsonRequestBehavior behavior)
        {
            return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding)
        {
            return new JsonNetResult
            {
                ContentEncoding = contentEncoding,
                ContentType = contentType,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = data
            };
        }

        protected JsonResult Json(string data)
        {
            return new JsonNetResult
            {
                ContentEncoding = null,
                ContentType = null,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                JsonData = data
            };
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