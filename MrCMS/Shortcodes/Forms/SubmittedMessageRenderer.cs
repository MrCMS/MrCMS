using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public class SubmittedMessageRenderer : ISubmittedMessageRenderer
    {
        public TagBuilder AppendSubmittedMessage(Webpage webpage, FormSubmittedStatus submittedStatus)
        {
            var message = new TagBuilder("div");
            message.AddCssClass("alert");
            message.AddCssClass(submittedStatus.Success ? "alert-success" : "alert-error");
            message.InnerHtml +=
                "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">x</button>" +
                (submittedStatus.Errors.Any()
                     ? RenderErrors(submittedStatus.Errors)
                     : (!string.IsNullOrWhiteSpace(webpage.FormSubmittedMessage)
                            ? webpage.FormSubmittedMessage
                            : "Form submitted"));

            return message;
        }

        private string RenderErrors(List<string> errors)
        {
            var tagBuilder = new TagBuilder("ul");
            errors.ForEach(error => tagBuilder.InnerHtml += string.Format("<li>{0}</li>", error));
            return tagBuilder.ToString();
        }
    }
}