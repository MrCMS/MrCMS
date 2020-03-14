using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Shortcodes.Forms;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Helpers;

namespace MrCMS.Shortcodes
{
    public class FormShortcodeRenderer : IShortcodeRenderer
    {
        private readonly IRepository<Form> _repository;
        private readonly IFormRenderer _formRenderer;

        public FormShortcodeRenderer(IRepository<Form> repository, IFormRenderer formRenderer)
        {
            _repository = repository;
            _formRenderer = formRenderer;
        }

        public string TagName => "form";

        public async Task<IHtmlContent> Render(IHtmlHelper helper, Dictionary<string, string> attributes)
        {
            if (!attributes.ContainsKey("id"))
            {
                return HtmlString.Empty;
            }

            if (!int.TryParse(attributes["id"], out var id))
            {
                return HtmlString.Empty;
            }

            var form = await _repository.GetData(id);
            if (form == null)
            {
                return HtmlString.Empty;
            }

            var status = GetStatus(helper.ViewContext);
            return await _formRenderer.RenderForm(helper, form, status);
        }

        private static FormSubmittedStatus GetStatus(ViewContext viewContext)
        {
            var submitted = true.Equals(viewContext.TempData["form-submitted"]);
            var errors = viewContext.TempData.Get<List<string>>("form-submitted-message");
            var data = viewContext.TempData.Get<NameValueCollection>("form-data");
            var status = new FormSubmittedStatus(submitted, errors, data);
            return status;
        }
    }
}