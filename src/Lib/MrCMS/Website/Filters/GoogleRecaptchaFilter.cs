using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace MrCMS.Website.Filters
{
    public class GoogleRecaptchaFilter : IActionFilter
    {
        // private readonly ICheckGoogleRecaptcha _checkGoogleRecaptcha;
        //
        // public GoogleRecaptchaFilter(ICheckGoogleRecaptcha checkGoogleRecaptcha)
        // {
        //     _checkGoogleRecaptcha = checkGoogleRecaptcha;
        // }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((filterContext.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo
                .GetCustomAttributes(typeof(GoogleRecaptchaAttribute), true).Any() != true)
            {
                return;
            }

            if (!filterContext.HttpContext.Request.HasFormContentType)
            {
                return;
            }

            string googleToken = filterContext.HttpContext.Request.Form["g-recaptcha-response"];

            var result = filterContext.HttpContext.RequestServices.GetRequiredService<ICheckGoogleRecaptcha>()
                .CheckToken(googleToken);
            switch (result)
            {
                case GoogleRecaptchaCheckResult.NotEnabled:
                case GoogleRecaptchaCheckResult.Success:
                    // continue
                    break;
                case GoogleRecaptchaCheckResult.Missing:
                    filterContext.Result = new ContentResult { Content = "Please Complete Recaptcha" };
                    break;
                case GoogleRecaptchaCheckResult.Failed:
                    filterContext.Result = new EmptyResult();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}