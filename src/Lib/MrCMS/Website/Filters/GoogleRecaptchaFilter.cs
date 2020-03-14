using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MrCMS.Website.Filters
{
    public class GoogleRecaptchaFilter : IAsyncActionFilter
    {
        private readonly ISystemConfigurationProvider _configurationProvider;

        public GoogleRecaptchaFilter(ISystemConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if ((context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttributes(typeof(GoogleRecaptchaAttribute), true).Any() != true)
            {
                await next();
                return;
            }

            var settings = await _configurationProvider.GetSystemSettings<GoogleRecaptchaSettings>();
            if (!settings.Enabled)
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
            if (string.IsNullOrWhiteSpace(googleToken))
            {
                var contentResult = new ContentResult { Content = "Please Complete Recaptcha" };
                context.Result = contentResult;
                return;
            }

            var data = new NameValueCollection
            {
                {"response", googleToken},
                {"secret", settings.Secret}
            };

            var googleResponse =
                new WebClient().UploadValues(new Uri("https://www.google.com/recaptcha/api/siteverify"),
                    "POST", data);
            var jsonString = Encoding.Default.GetString(googleResponse);
            var json = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(jsonString);
            if (!json.Success)
            {
                context.Result = new EmptyResult();
                return;
            }
            await next();
        }
    }
}