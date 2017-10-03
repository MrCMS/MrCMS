using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using MrCMS.Settings;
using Newtonsoft.Json;

namespace MrCMS.Website.Filters
{
    public class GoogleRecaptchaFilter : IActionFilter
    {
        private readonly GoogleRecaptchaSettings _settings;

        public GoogleRecaptchaFilter(GoogleRecaptchaSettings settings)
        {
            _settings = settings;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionDescriptor.GetCustomAttributes(typeof(GoogleRecaptchaAttribute), true).Any())
                return;
            if (!CurrentRequestData.DatabaseIsInstalled)
                return;
            if (!_settings.Enabled)
                return;

            string googleToken = filterContext.HttpContext.Request["g-recaptcha-response"];
            if (string.IsNullOrWhiteSpace(googleToken))
            {
                var contentResult = new ContentResult { Content = "Please Complete Recaptcha" };
                filterContext.Result = contentResult;
            }
            else
            {
                var data = new NameValueCollection
                {
                    {"response", googleToken},
                    {"secret", _settings.Secret}
                };

                var googleResponse =
                    new WebClient().UploadValues(new Uri("https://www.google.com/recaptcha/api/siteverify"),
                        "POST", data);
                var jsonString = Encoding.Default.GetString(googleResponse);
                var json = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(jsonString);
                if (!json.Success)
                    filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}