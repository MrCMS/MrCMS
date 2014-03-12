using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MrCMS.Helpers
{
    public static class HtmlFormExtensions
    {
        public static MvcForm BeginForm(this HtmlHelper htmlHelper, FormMethod formMethod, object htmlAttributes)
        {
            // generates <form action="{current url}" method="post">...</form> 
            string formAction = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            return FormHelper(htmlHelper, formAction, formMethod, MrCMSHtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification =
                "Because disposing the object would write to the response stream, you don't want to prematurely dispose of this object."
            )]
        private static MvcForm FormHelper(this HtmlHelper htmlHelper, string formAction, FormMethod method,
                                          IDictionary<string, object> htmlAttributes)
        {
            var tagBuilder = new TagBuilder("form");
            tagBuilder.MergeAttributes(htmlAttributes);
            // action is implicitly generated, so htmlAttributes take precedence.
            tagBuilder.MergeAttribute("action", formAction);
            // method is an explicit parameter, so it takes precedence over the htmlAttributes. 
            tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(method), true);

            htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            var theForm = new MvcForm(htmlHelper.ViewContext);

            return theForm;
        }
    }
}