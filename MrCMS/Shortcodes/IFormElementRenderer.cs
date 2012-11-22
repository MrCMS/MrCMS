using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
{
    public interface IFormElementRenderer
    {
        void AppendTextBox(dynamic element, TagBuilder formBuilder);
        void AppendTextArea(dynamic element, TagBuilder formBuilder);
        void AppendCheckboxList(dynamic element, TagBuilder formBuilder);
        void AppendRadioButtons(dynamic element, TagBuilder formBuilder);
        void AppendSelectList(dynamic element, TagBuilder formBuilder);
        void AppendSubmittedMessage(HtmlHelper htmlHelper, TagBuilder formBuilder, Webpage page);
        void AppendSubmitButton(TagBuilder formBuilder);
    }
}