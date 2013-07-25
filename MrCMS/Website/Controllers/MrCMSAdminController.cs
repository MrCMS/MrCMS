using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MrCMS.Entities.People;
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
                //// loop over the existing parameters
                //var collection = filterContext.ActionParameters.Keys.ToList();
                //foreach (var key in collection)
                //{
                //    // see if a value has been persisted to temp data as part of an invalid model
                //    var value = TempData["parameters-" + key];
                //    // if one is found, set it
                //    if (value != null)
                //        filterContext.ActionParameters[key] = value;
                //}

                // merge in any invalid modelstate found
                var modelStateDictionary = TempData["MrCMS-invalid-modelstate"] as ModelStateDictionary;
                if (modelStateDictionary != null)
                {
                    ModelState.Merge(modelStateDictionary);
                }
            }

            // HTTP POST
            // if model state is invalid and it's a post
            if (!ModelState.IsValid && filterContext.HttpContext.Request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                //// copy the parameter values into tempdata
                //foreach (var actionParameter in filterContext.ActionParameters)
                //{
                //    TempData["parameters-" + actionParameter.Key] = actionParameter.Value;
                //}
                // persist the model state to the tempdata dictionary
                TempData["MrCMS-invalid-modelstate"] = ModelState;
                // redirect to the previous page
                filterContext.Result = new RedirectResult(Referrer.ToString());
            }

            ViewData["controller-name"] = ControllerContext.RouteData.Values["controller"];

            if (MrCMSApplication.Get<SiteSettings>().SSLAdmin)
            {
                if (!Request.IsSecureConnection && !Request.IsLocal)
                {
                    Response.Redirect(Request.Url.ToString().Replace("http://", "https://"));
                }
            }

            base.OnActionExecuting(filterContext);
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