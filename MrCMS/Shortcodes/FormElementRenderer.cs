using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Shortcodes
{
    public class FormElementRenderer : IFormElementRenderer
    {
        public virtual void AppendTextBox(dynamic element, TagBuilder formBuilder)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes["for"] = MakeId(element.values);
            labelBuilder.InnerHtml = element.values;
            formBuilder.InnerHtml += labelBuilder.ToString();

            var textBuilder = new TagBuilder("input");
            textBuilder.Attributes["type"] = "text";
            textBuilder.Attributes["id"] = MakeId(element.values);
            textBuilder.Attributes["name"] = element.values;
            textBuilder.AddCssClass(element.itemclass.ToString());
            if (element.required == "checked")
            {
                textBuilder.Attributes["data-val"] = true.ToString();
                textBuilder.Attributes["data-val-required"] = element.values + " is required.";
                textBuilder.AddCssClass("required");
            }
            formBuilder.InnerHtml += textBuilder.ToString();
        }

        private string MakeId(dynamic value)
        {
            return value.ToString().ToLower().Replace(" ", "_");
        }

        public virtual void AppendTextArea(dynamic element, TagBuilder formBuilder)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes["for"] = MakeId(element.values);
            labelBuilder.InnerHtml = element.values;
            formBuilder.InnerHtml += labelBuilder.ToString();

            var textBuilder = new TagBuilder("textarea");
            textBuilder.Attributes["id"] = MakeId(element.values);
            textBuilder.Attributes["name"] = element.values;
            textBuilder.AddCssClass(element.itemclass.ToString());
            if (element.required == "checked")
            {
                textBuilder.Attributes["data-val"] =
                    true.ToString();
                textBuilder.Attributes["data-val-required"] = element.values + " is required.";
                textBuilder.AddCssClass("required");
            }
            formBuilder.InnerHtml += textBuilder.ToString();
        }

        public virtual void AppendCheckboxList(dynamic element, TagBuilder formBuilder)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes["for"] = element.title;
            labelBuilder.InnerHtml = element.title;
            formBuilder.InnerHtml += labelBuilder.ToString();

            foreach (dynamic checkbox in element.values)
            {
                var checkboxBuilder = new TagBuilder("input");
                checkboxBuilder.Attributes["type"] = "checkbox";
                checkboxBuilder.Attributes["value"] = checkbox.value;
                checkboxBuilder.AddCssClass(element.itemclass.ToString());

                if (checkbox.baseline == true)
                    checkboxBuilder.Attributes["checked"] = "checked";

                checkboxBuilder.Attributes["name"] = element.title;
                checkboxBuilder.Attributes["id"] = MakeId(element.title + "_" + checkbox.value);
                formBuilder.InnerHtml += checkboxBuilder.ToString();

                var cbLabelBuilder = new TagBuilder("label");
                cbLabelBuilder.Attributes["for"] = MakeId(element.title + "_" + checkbox.value);
                cbLabelBuilder.InnerHtml = checkbox.value;
                formBuilder.InnerHtml += cbLabelBuilder.ToString();
            }
        }

        public virtual void AppendRadioButtons(dynamic element, TagBuilder formBuilder)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes["for"] = element.title;
            labelBuilder.InnerHtml = element.title;
            formBuilder.InnerHtml += labelBuilder.ToString();

            foreach (var radioButton in element.values)
            {
                var radioButtonBuilder = new TagBuilder("input");
                radioButtonBuilder.Attributes["type"] = "radio";
                radioButtonBuilder.Attributes["value"] = radioButton.value;
                radioButtonBuilder.AddCssClass(element.itemclass.ToString());

                if (radioButton.baseline == true)
                    radioButtonBuilder.Attributes["checked"] = "checked";

                radioButtonBuilder.Attributes["name"] = element.title;
                radioButtonBuilder.Attributes["id"] = MakeId(element.title + "_" + radioButton.value);
                formBuilder.InnerHtml += radioButtonBuilder.ToString();

                var cbLabelBuilder = new TagBuilder("label");
                cbLabelBuilder.Attributes["for"] = MakeId(element.title + "_" + radioButton.value);
                cbLabelBuilder.InnerHtml = radioButton.value;
                formBuilder.InnerHtml += cbLabelBuilder.ToString();
            }
        }

        public virtual void AppendSelectList(dynamic element, TagBuilder formBuilder)
        {
            var labelBuilder = new TagBuilder("label");
            labelBuilder.Attributes["for"] = MakeId(element.title);
            labelBuilder.InnerHtml = element.title;
            formBuilder.InnerHtml += labelBuilder.ToString();

            var selectTagBuilder = new TagBuilder("select");
            selectTagBuilder.Attributes["name"] = element.title;
            selectTagBuilder.Attributes["id"] = MakeId(element.title);
            selectTagBuilder.AddCssClass(element.itemclass.ToString());
            foreach (var option in element.values)
            {
                var optionBuilder = new TagBuilder("option") { InnerHtml = option.value };

                if (option.baseline == true)
                    optionBuilder.Attributes["selected"] = "selected";

                selectTagBuilder.InnerHtml += optionBuilder.ToString();
            }
            formBuilder.InnerHtml += selectTagBuilder.ToString();
        }

        public virtual void AppendSubmittedMessage(HtmlHelper htmlHelper, TagBuilder formBuilder, Webpage page)
        {
            if (true.Equals(htmlHelper.ViewContext.TempData["form-submitted"]))
            {
                var message = new TagBuilder("div");
                message.AddCssClass("alert");
                message.AddCssClass("alert-success");
                message.InnerHtml +=
                    "<button type=\"button\" class=\"close\" data-dismiss=\"alert\">×</button>" +
                    (!string.IsNullOrWhiteSpace(page.FormSubmittedMessage) ? page.FormSubmittedMessage : "Form submitted");

                formBuilder.InnerHtml += message.ToString();
            }
        }

        public virtual void AppendSubmitButton(TagBuilder formBuilder)
        {
            var submitBuilder = new TagBuilder("input");
            submitBuilder.Attributes["type"] = "submit";
            submitBuilder.Attributes["value"] = "Submit";
            submitBuilder.AddCssClass("btn btn-primary");

            formBuilder.InnerHtml += string.Format("<div>{0}</div>", submitBuilder);
        }
    }
}