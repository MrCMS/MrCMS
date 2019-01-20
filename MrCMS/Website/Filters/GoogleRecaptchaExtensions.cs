using System;
using System.Web.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public static class GoogleRecaptchaExtensions
    {
        public static MvcHtmlString RenderRecaptcha(this IHtmlHelper helper, string id = null, string errorClass = null,
            string errorMessage = null)
        {
            var settings = MrCMSApplication.Get<GoogleRecaptchaSettings>();

            return RenderDiv(id, errorClass, errorMessage, settings);
        }

        public static MvcHtmlString RenderRecaptcha(this HtmlHelper helper, string id = null, string errorClass = null,
            string errorMessage = null)
        {
            var settings = MrCMSApplication.Get<GoogleRecaptchaSettings>();

            return RenderDiv(id, errorClass, errorMessage, settings);
        }

        private static MvcHtmlString RenderDiv(string id, string errorClass, string errorMessage,
            GoogleRecaptchaSettings settings)
        {
            if (!settings.Enabled)
                return MvcHtmlString.Empty;
            var recaptchaDiv = new TagBuilder("div");
            recaptchaDiv.AddCssClass("g-recaptcha");
            recaptchaDiv.AddCssClass("mb-3");
            recaptchaDiv.Attributes["data-recaptcha-holder"] = "true";
            recaptchaDiv.Attributes["data-sitekey"] = settings.SiteKey;
            id = id ?? Guid.NewGuid().ToString();
            recaptchaDiv.Attributes["id"] = id;

            var message = new TagBuilder("span");
            message.Attributes["data-recaptcha-message-for"] = id;
            message.AddCssClass(errorClass ?? "field-validation-error");
            message.Attributes["data-error-message"] = errorMessage ?? "Please fill in the reCAPTCHA before submitting";

            return MvcHtmlString.Create(string.Concat(recaptchaDiv.ToString(), message.ToString()));
        }
    }
}