using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.Filters
{
    public class GoogleRecaptchaFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                         ActionExecutionDelegate next)
        {
            if ((context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo
                .GetCustomAttributes(typeof(GoogleRecaptchaAttribute), true).Any() != true)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.HasFormContentType)
            {
                await next();
                return;
            }

            string googleToken = context.HttpContext.Request.Form["g-recaptcha-response"];

            var result = await context.HttpContext.RequestServices.GetRequiredService<ICheckGoogleRecaptcha>()
                .CheckTokenAsync(googleToken);
            switch (result)
            {
                case GoogleRecaptchaCheckResult.NotEnabled:
                case GoogleRecaptchaCheckResult.Success:
                    // continue
                    break;
                case GoogleRecaptchaCheckResult.Missing:
                    context.Result = new ContentResult { Content = "Please Complete Recaptcha" };
                    break;
                case GoogleRecaptchaCheckResult.Failed:
                    context.Result = new EmptyResult();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await next();
        }
    }
}