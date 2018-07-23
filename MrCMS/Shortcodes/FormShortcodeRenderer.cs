using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Shortcodes.Forms;
using NHibernate;

namespace MrCMS.Shortcodes
{
    public class FormShortcodeRenderer : IShortcodeRenderer
    {
        private readonly IFormRenderer _formRenderer;
        private readonly ISession _session;

        public FormShortcodeRenderer(ISession session, IFormRenderer formRenderer)
        {
            _session = session;
            _formRenderer = formRenderer;
        }

        public string TagName => "form";

        public string Render(IHtmlHelper helper, Dictionary<string, string> attributes)
        {
            Webpage page = null;
            if (attributes.ContainsKey("id"))
            {
                int id;
                if (!int.TryParse(attributes["id"], out id))
                    return string.Empty;
                page = _session.Get<Webpage>(id);
            }
            else
            {
                // TODO: get current page
                //page =  CurrentRequestData.CurrentPage;
            }

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