using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public static class GoogleRecaptchaExtensions
    {
        public static IHtmlContent RenderRecaptcha(this IHtmlHelper helper, string id = null, string errorClass = null,
            string errorMessage = null)
        {
            var settings = helper.GetRequiredService<GoogleRecaptchaSettings>();

            return RenderDiv(id, errorClass, errorMessage, settings);
        }

        private static IHtmlContent RenderDiv(string id, string errorClass, string errorMessage,
            GoogleRecaptchaSettings settings)
        {
            if (!settings.Enabled)
                return HtmlString.Empty;
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

            return new HtmlString(string.Concat(recaptchaDiv.ToString(), message.ToString()));
        }
    }
}