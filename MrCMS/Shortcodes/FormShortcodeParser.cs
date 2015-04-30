using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Shortcodes.Forms;
using MrCMS.Website;

namespace MrCMS.Shortcodes
{
    public class FormShortcodeParser : IShortcodeParser
    {
        private static readonly Regex OtherPageFormMatch = new Regex(@"\[form-(\d+)\]", RegexOptions.Compiled);
        private static readonly Regex ThisPageFormMatch = new Regex(@"\[form\]", RegexOptions.Compiled);

        private readonly IDocumentService _documentService;
        private readonly IFormRenderer _formRenderer;

        public FormShortcodeParser(IDocumentService documentService, IFormRenderer formRenderer)
        {
            _documentService = documentService;
            _formRenderer = formRenderer;
        }

        public string Parse(HtmlHelper htmlHelper, string current)
        {
            if (current == null)
                return null;

            bool submitted = true.Equals(htmlHelper.ViewContext.TempData["form-submitted"]);
            var errors = htmlHelper.ViewContext.TempData["form-submitted-message"] as List<string>;
            var data = htmlHelper.ViewContext.TempData["form-data"] as NameValueCollection;
            var status = new FormSubmittedStatus(submitted, errors, data);
            current = OtherPageFormMatch.Replace(current,
                match =>
                {
                    int pageId = Convert.ToInt32(
                        match.Value.Replace("[", "").Replace("]", "").Split('-')[1]);
                    var document = _documentService.GetDocument<Webpage>(pageId);
                    return document == null
                        ? string.Empty
                        : _formRenderer.RenderForm(document, status);
                });

            current = ThisPageFormMatch.Replace(current,
                match => _formRenderer.RenderForm(CurrentRequestData.CurrentPage, status));

            return current;
        }

        //private string GetForm(HtmlHelper htmlHelper, string formData, Webpage page)
        //{
        //    dynamic deserializedObject =
        //        JsonConvert.DeserializeObject(formData);

        //    var formStructure = deserializedObject.form_structure;
        //    if (formStructure != null)
        //    {
        //        var formBuilder = new TagBuilder("form");
        //        formBuilder.Attributes["action"] =
        //            string.Format("/save-form/{0}", page.Id);
        //        formBuilder.Attributes["method"] = "POST";

        //        foreach (dynamic element in formStructure)
        //        {
        //            if (element.cssClass == "input_text")
        //                _formElementRenderer.AppendTextBox(element, formBuilder);
        //            else if (element.cssClass == "textarea")
        //                _formElementRenderer.AppendTextArea(element, formBuilder);
        //            else if (element.cssClass == "checkbox")
        //                _formElementRenderer.AppendCheckboxList(element, formBuilder);
        //            else if (element.cssClass == "radio")
        //                _formElementRenderer.AppendRadioButtons(element, formBuilder);
        //            else if (element.cssClass == "select")
        //                _formElementRenderer.AppendSelectList(element, formBuilder);
        //        }

        //        _formElementRenderer.AppendSubmitButton(formBuilder);

        //        _formElementRenderer.AppendSubmittedMessage(htmlHelper, formBuilder, page);
        //        return formBuilder.ToString();
        //    }
        //    return string.Empty;
        //}
    }
}