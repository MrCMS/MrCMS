using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Website;
using Newtonsoft.Json;

namespace MrCMS.Shortcodes
{
    public class FormShortcodeParser : IShortcodeParser
    {
        private readonly IDocumentService _documentService;
        private readonly IFormService _formService;
        private readonly IFormElementRenderer _formElementRenderer;

        public FormShortcodeParser(IDocumentService documentService, IFormService formService, IFormElementRenderer formElementRenderer)
        {
            _documentService = documentService;
            _formService = formService;
            _formElementRenderer = formElementRenderer;
        }

        public string Parse(HtmlHelper htmlHelper, string current)
        {
            if (current == null)
                return null;

            var otherPageFormMatch = new Regex(@"\[form-[\d+]\]");
            var thisPageFormMatch = new Regex(@"\[form\]");

            current = otherPageFormMatch.Replace(current,
                                                 match =>
                                                 {
                                                     var pageId = Convert.ToInt32(
                                                         match.Value.Replace("[", "").Replace("]", "").Split('-')[1]);
                                                     var document = _documentService.GetDocument<Webpage>(pageId);
                                                     var formData = _formService.GetFormStructure(document);
                                                     return document == null
                                                                ? string.Empty
                                                                : GetForm(htmlHelper, formData,
                                                                          document);
                                                 });

            current = thisPageFormMatch.Replace(current,
                                                match =>
                                                {
                                                    var formData =
                                                        _formService.GetFormStructure(
                                                            CurrentRequestData.CurrentPage);
                                                    return GetForm(htmlHelper, formData,
                                                                   CurrentRequestData.CurrentPage);
                                                });

            return current;
        }

        private string GetForm(HtmlHelper htmlHelper, string formData, Webpage page)
        {
            dynamic deserializedObject =
                JsonConvert.DeserializeObject(formData);

            var formStructure = deserializedObject.form_structure;
            if (formStructure != null)
            {
                var formBuilder = new TagBuilder("form");
                formBuilder.Attributes["action"] =
                    string.Format("/save-form/{0}", page.Id);
                formBuilder.Attributes["method"] = "POST";

                foreach (dynamic element in formStructure)
                {
                    if (element.cssClass == "input_text")
                        _formElementRenderer.AppendTextBox(element, formBuilder);
                    else if (element.cssClass == "textarea")
                        _formElementRenderer.AppendTextArea(element, formBuilder);
                    else if (element.cssClass == "checkbox")
                        _formElementRenderer.AppendCheckboxList(element, formBuilder);
                    else if (element.cssClass == "radio")
                        _formElementRenderer.AppendRadioButtons(element, formBuilder);
                    else if (element.cssClass == "select")
                        _formElementRenderer.AppendSelectList(element, formBuilder);
                }

                _formElementRenderer.AppendSubmitButton(formBuilder);

                _formElementRenderer.AppendSubmittedMessage(htmlHelper, formBuilder, page);
                return formBuilder.ToString();
            }
            return string.Empty;
        }
    }
}