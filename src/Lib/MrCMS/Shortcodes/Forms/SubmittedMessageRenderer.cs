using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes.Forms
{
    public class SubmittedMessageRenderer : ISubmittedMessageRenderer
    {
        public TagBuilder AppendSubmittedMessage(Form form, FormSubmittedStatus submittedStatus)
        {
            var message = new TagBuilder("div");
            message.AddCssClass("alert");
            message.AddCssClass(submittedStatus.Success ? "alert-success" : "alert-danger");
            message.InnerHtml.AppendHtml(
                "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">x</button>" +
                (submittedStatus.Errors.Any()
                    ? RenderErrors(submittedStatus.Errors)
                    : (!string.IsNullOrWhiteSpace(form.FormSubmittedMessage)
                        ? form.FormSubmittedMessage
                        : "Form submitted")));

            return message;
        }

        private string RenderErrors(List<string> errors)
        {
            var tagBuilder = new TagBuilder("ul");
            errors.ForEach(error => tagBuilder.InnerHtml.AppendHtml(string.Format("<li>{0}</li>", error)));
            return tagBuilder.ToString();
        }
    }
}