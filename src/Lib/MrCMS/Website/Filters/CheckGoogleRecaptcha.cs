using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using MrCMS.Settings;
using Newtonsoft.Json;

namespace MrCMS.Website.Filters
{
    public class CheckGoogleRecaptcha : ICheckGoogleRecaptcha
    {
        private readonly GoogleRecaptchaSettings _settings;

        public CheckGoogleRecaptcha(GoogleRecaptchaSettings settings)
        {
            _settings = settings;
        }

        public GoogleRecaptchaCheckResult CheckToken(string token)
        {
            if (!_settings.Enabled)
            {
                return GoogleRecaptchaCheckResult.NotEnabled;
            }


            if (string.IsNullOrWhiteSpace(token))
            {
                // var contentResult = new ContentResult {Content = "Please Complete Recaptcha"};
                return GoogleRecaptchaCheckResult.Missing;
            }

            var data = new NameValueCollection
            {
                {"response", token},
                {"secret", _settings.Secret}
            };

            var googleResponse =
                new WebClient().UploadValues(new Uri("https://www.google.com/recaptcha/api/siteverify"),
                    "POST", data);
            var jsonString = Encoding.Default.GetString(googleResponse);
            var json = JsonConvert.DeserializeObject<GoogleRecaptchaResponse>(jsonString);
            if (!json.Success)
            {
                return GoogleRecaptchaCheckResult.Failed;
            }

            return GoogleRecaptchaCheckResult.Success;
        }
    }
}