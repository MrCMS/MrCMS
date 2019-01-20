using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Shortcodes.Forms;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Shortcodes
{
    public class FormShortcodeRenderer : IShortcodeRenderer
    {
        private readonly ISession _session;
        private readonly IFormRenderer _formRenderer;

        public FormShortcodeRenderer(ISession session, IFormRenderer formRenderer)
        {
            _session = session;
            _formRenderer = formRenderer;
        }

        public string TagName => "form";

        public string Render(IHtmlHelper helper, Dictionary<string, string> attributes)
        {
            Webpage page;
            if (attributes.ContainsKey("id"))
            {
                int id;
                if (!int.TryParse(attributes["id"], out id))
                    return string.Empty;
                page = _session.Get<Webpage>(id);
            }
            else
                page = CurrentRequestData.CurrentPage;

            var status = GetStatus(helper.ViewContext);
            return _formRenderer.RenderForm(helper, page, status);
        }

        private static FormSubmittedStatus GetStatus(ViewContext viewContext)
        {
            var submitted = true.Equals(viewContext.TempData["form-submitted"]);
            var errors = viewContext.TempData["form-submitted-message"] as List<string>;
            var data = viewContext.TempData["form-data"] as NameValueCollection;
            var status = new FormSubmittedStatus(submitted, errors, data);
            return status;
        }
    }
}